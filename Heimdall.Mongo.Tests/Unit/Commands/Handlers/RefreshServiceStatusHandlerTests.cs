using Heimdall.Core.Commands;
using Heimdall.Mongo.Commands.Handlers;
using Heimdall.Mongo.Infrastructure;
using Heimdall.Mongo.Tests.Common.Utils;
using LibCore.CQRS.Validation;
using LibCore.Web.Services;
using MediatR;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Heimdall.Mongo.Tests.Unit.Commands.Handlers
{
    public class RefreshServiceStatusHandlerTests
    {
        [Fact]
        public void should_throw_ArgumentNullException_when_dbContext_null()
        {
            var mockValidator = new Mock<IValidator<RefreshServiceStatus>>();
            var mockPinger = new Mock<IPinger>();
            var mockMediator = new Mock<IMediator>();
            Assert.Throws<ArgumentNullException>(() => new RefreshServiceStatusHandler(null, mockPinger.Object, mockMediator.Object, mockValidator.Object));
        }

        [Fact]
        public void should_throw_ArgumentNullException_when_pinger_null()
        {
            var mockValidator = new Mock<IValidator<RefreshServiceStatus>>();
            var mockDbContext = new Mock<IDbContext>();
            var mockMediator = new Mock<IMediator>();
            Assert.Throws<ArgumentNullException>(() => new RefreshServiceStatusHandler(mockDbContext.Object, null, mockMediator.Object, mockValidator.Object));
        }

        [Fact]
        public void should_throw_ArgumentNullException_when_mediator_null()
        {
            var mockValidator = new Mock<IValidator<RefreshServiceStatus>>();
            var mockDbContext = new Mock<IDbContext>();
            var mockPinger = new Mock<IPinger>();
            Assert.Throws<ArgumentNullException>(() => new RefreshServiceStatusHandler(mockDbContext.Object, mockPinger.Object, null, mockValidator.Object));
        }

        [Fact]
        public async Task should_fail_when_command_null()
        {
            var mockDbContext = new Mock<IDbContext>();
            var mockPinger = new Mock<IPinger>();
            var mockValidator = new Mock<IValidator<RefreshServiceStatus>>();
            var mockMediator = new Mock<IMediator>();

            var sut = new RefreshServiceStatusHandler(mockDbContext.Object, mockPinger.Object, mockMediator.Object, mockValidator.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(null));
        }

        [Fact]
        public async Task should_deactivate_service_when_no_endpoints_available()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = true,
                Name = "lorem",
                Endpoints = Enumerable.Empty<Mongo.Infrastructure.Entities.ServiceEndpoint>()
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var mockPinger = new Mock<IPinger>();
            var mockMediator = new Mock<IMediator>();
            var validator = new NullValidator<RefreshServiceStatus>();

            var sut = new RefreshServiceStatusHandler(mockDbContext.Object, mockPinger.Object, mockMediator.Object, validator);
            await sut.Handle(new RefreshServiceStatus(service.Name, 10));

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Mongo.Infrastructure.Entities.Service, bool>>>(),
                                                  It.Is<Mongo.Infrastructure.Entities.Service>(r => r.Active == false)),
                            Times.Once());
        }

        [Fact]
        public async Task should_deactivate_service_when_no_endpoints_responds_to_ping()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = true,
                Name = "lorem",
                Endpoints = new[]{
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = true,
                        Url = "localhost"
                    }
                }
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var mockPinger = new Mock<IPinger>();
            mockPinger.Setup(p => p.PingAsync(It.IsAny<string>(), It.IsAny<int>()))
                      .ReturnsAsync(new PingResult(false, 0));

            var mockMediator = new Mock<IMediator>();

            var validator = new NullValidator<RefreshServiceStatus>();

            var command = new RefreshServiceStatus(service.Name, 10);
            var sut = new RefreshServiceStatusHandler(mockDbContext.Object, mockPinger.Object, mockMediator.Object, validator);
            await sut.Handle(command);

            mockPinger.Verify(m => m.PingAsync(service.Endpoints.ElementAt(0).Url, command.Timeout), Times.Once());

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Mongo.Infrastructure.Entities.Service, bool>>>(),
                                                  It.Is<Mongo.Infrastructure.Entities.Service>(r => r.Active == false && !r.Endpoints.Any(es => es.Active))),
                            Times.Once());
        }

        [Fact]
        public async Task should_activate_service_when_at_least_one_endpoint_responds_to_ping()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = false,
                Name = "lorem",
                Endpoints = new[]{
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = false,
                        Url = "localhost1"
                    },
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = false,
                        Url = "localhost2"
                    }
                }
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var mockPinger = new Mock<IPinger>();
            mockPinger.Setup(p => p.PingAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((string url, int timeout) =>
                {
                    return new PingResult((url == "localhost2"), 0);
                });

            var mockMediator = new Mock<IMediator>();

            var validator = new NullValidator<RefreshServiceStatus>();

            var command = new RefreshServiceStatus(service.Name, 10);
            var sut = new RefreshServiceStatusHandler(mockDbContext.Object, mockPinger.Object, mockMediator.Object, validator);
            await sut.Handle(command);

            foreach (var endpoint in service.Endpoints)
                mockPinger.Verify(m => m.PingAsync(endpoint.Url, command.Timeout), Times.Once());

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Mongo.Infrastructure.Entities.Service, bool>>>(),
                                                  It.Is<Mongo.Infrastructure.Entities.Service>(r => r.Active == true && 1 == r.Endpoints.Count(es => es.Active && es.Url == "localhost2"))),
                            Times.Once());
        }

        [Fact]
        public async Task should_update_endpoint_roundtrip_time()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = false,
                Name = "lorem",
                Endpoints = new[]{
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = false,
                        Url = "localhost",
                        RoundtripTime = long.MaxValue
                    }
                }
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var mockPinger = new Mock<IPinger>();
            mockPinger.Setup(p => p.PingAsync(It.IsAny<string>(), It.IsAny<int>()))
                      .ReturnsAsync(new PingResult(true, 42));

            var mockMediator = new Mock<IMediator>();

            var validator = new NullValidator<RefreshServiceStatus>();

            var command = new RefreshServiceStatus(service.Name, 10);
            var sut = new RefreshServiceStatusHandler(mockDbContext.Object, mockPinger.Object, mockMediator.Object, validator);
            await sut.Handle(command);

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Mongo.Infrastructure.Entities.Service, bool>>>(),
                                                  It.Is<Mongo.Infrastructure.Entities.Service>(r => r.Endpoints.Any(es => es.RoundtripTime == 42))),
                            Times.Once());
        }

        [Fact]
        public async Task should_ping_all_endpoints_even_with_exceptions()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = true,
                Name = "lorem",
                Endpoints = new[]{
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = true,
                        Url = "localhost"
                    },
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = true,
                        Url = "localhost1"
                    }
                }
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var mockMediator = new Mock<IMediator>();

            var mockPinger = new Mock<IPinger>();
            mockPinger.Setup(p => p.PingAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((string url, int timeout) =>
                {
                    if(url == service.Endpoints.ElementAt(0).Url)
                        throw new Exception(url);
                    return new PingResult(true, 0);
                });

            var validator = new NullValidator<RefreshServiceStatus>();

            var command = new RefreshServiceStatus(service.Name, 10);
            var sut = new RefreshServiceStatusHandler(mockDbContext.Object, mockPinger.Object, mockMediator.Object, validator);
            await sut.Handle(command);

            mockPinger.Verify(m => m.PingAsync(It.IsAny<string>(), command.Timeout), Times.Exactly(service.Endpoints.Count()));
        }

        [Fact]
        public async Task should_emit_ServiceRefreshed_event()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = true,
                Name = "lorem",
                Endpoints = new[]{
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = true,
                        Url = "localhost"
                    },
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = true,
                        Url = "localhost1"
                    }
                }
            };

            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var mockMediator = new Mock<IMediator>();

            var mockPinger = new Mock<IPinger>();

            var validator = new NullValidator<RefreshServiceStatus>();

            var command = new RefreshServiceStatus(service.Name, 10);
            var sut = new RefreshServiceStatusHandler(mockDbContext.Object, mockPinger.Object, mockMediator.Object, validator);
            await sut.Handle(command);

            mockMediator.Verify(m => m.Publish(It.Is<Core.Events.ServiceRefreshed>(e => e.ServiceId == service.Id), 
                                                It.IsAny<System.Threading.CancellationToken>()), 
                                Times.Once);
        }

        [Fact]
        public async Task should_emit_ServiceRefreshed_event_even_with_no_endpoints()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = true,
                Name = "lorem",
                Endpoints = new Mongo.Infrastructure.Entities.ServiceEndpoint[]{ }
            };

            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var mockMediator = new Mock<IMediator>();

            var mockPinger = new Mock<IPinger>();

            var validator = new NullValidator<RefreshServiceStatus>();

            var command = new RefreshServiceStatus(service.Name, 10);
            var sut = new RefreshServiceStatusHandler(mockDbContext.Object, mockPinger.Object, mockMediator.Object, validator);
            await sut.Handle(command);

            mockMediator.Verify(m => m.Publish(It.Is<Core.Events.ServiceRefreshed>(e => e.ServiceId == service.Id),
                                                It.IsAny<System.Threading.CancellationToken>()),
                                Times.Once);
        }
    }
}
