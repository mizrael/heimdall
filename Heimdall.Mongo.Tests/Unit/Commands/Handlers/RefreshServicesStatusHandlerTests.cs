﻿using Heimdall.Core.Commands;
using Heimdall.Mongo.Commands.Handlers;
using Heimdall.Mongo.Infrastructure;
using LibCore.CQRS.Validation;
using LibCore.Mongo;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using LibCore.Web.Services;
using Heimdall.Mongo.Tests.Utils;

namespace Heimdall.Mongo.Tests.Unit.Commands.Handlers
{
    public class RefreshServicesStatusHandlerTests
    {
        [Fact]
        public void should_throw_ArgumentNullException_when_dbContext_null()
        {
            var mockValidator = new Mock<IValidator<RefreshServicesStatus>>();
            var mockPinger = new Mock<IPinger>();
            Assert.Throws<ArgumentNullException>(() => new RefreshServicesStatusHandler(null, mockPinger.Object, mockValidator.Object));
        }

        [Fact]
        public void should_throw_ArgumentNullException_when_pinger_null()
        {
            var mockValidator = new Mock<IValidator<RefreshServicesStatus>>();
            var mockDbContext = new Mock<IDbContext>();
            Assert.Throws<ArgumentNullException>(() => new RefreshServicesStatusHandler(mockDbContext.Object, null, mockValidator.Object));
        }

        [Fact]
        public async Task should_fail_when_command_null()
        {
            var mockDbContext = new Mock<IDbContext>();
            var mockPinger = new Mock<IPinger>();
            var mockValidator = new Mock<IValidator<RefreshServicesStatus>>();

            var sut = new RefreshServicesStatusHandler(mockDbContext.Object, mockPinger.Object, mockValidator.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(null));
        }

        [Fact]
        public async Task should_deactivate_service_when_no_endpoints_available()
        {
            var service = new Infrastructure.Entities.Service()
            {
                Active = true,
                Name = "lorem",
                Endpoints = Enumerable.Empty<Infrastructure.Entities.ServiceEndpoint>()
            };
            var mockRepo = RepositoryUtils.MockRepository<Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var mockPinger = new Mock<IPinger>();

            var validator = new NullValidator<RefreshServicesStatus>();

            var sut = new RefreshServicesStatusHandler(mockDbContext.Object, mockPinger.Object, validator);
            await sut.Handle(new RefreshServicesStatus(10));
            
            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Infrastructure.Entities.Service, bool>>>(),
                                                  It.Is<Infrastructure.Entities.Service>(r => r.Active == false)), 
                            Times.Once());
        }

        [Fact]
        public async Task should_deactivate_service_when_no_endpoints_responds_to_ping()
        {
            var service = new Infrastructure.Entities.Service()
            {
                Active = true,
                Name = "lorem",
                Endpoints = new[]{
                    new Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = true,
                        Url = "localhost"
                    }
                }
            };
            var mockRepo = RepositoryUtils.MockRepository<Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var mockPinger = new Mock<IPinger>();
            mockPinger.Setup(p => p.PingAsync(It.IsAny<string>(), It.IsAny<int>()))
                      .ReturnsAsync(new PingResult(false, 0));

            var validator = new NullValidator<RefreshServicesStatus>();

            var command = new RefreshServicesStatus(10);
            var sut = new RefreshServicesStatusHandler(mockDbContext.Object, mockPinger.Object, validator);
            await sut.Handle(command);

            mockPinger.Verify(m => m.PingAsync(service.Endpoints.ElementAt(0).Url, command.Timeout), Times.Once());

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Infrastructure.Entities.Service, bool>>>(),
                                                  It.Is<Infrastructure.Entities.Service>(r => r.Active == false && !r.Endpoints.Any(es => es.Active)) ),
                            Times.Once());
        }

        [Fact]
        public async Task should_activate_service_when_at_least_one_endpoint_responds_to_ping()
        {
            var service = new Infrastructure.Entities.Service()
            {
                Active = false,
                Name = "lorem",
                Endpoints = new[]{
                    new Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = false,
                        Url = "localhost1"
                    },
                    new Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = false,
                        Url = "localhost2"
                    }
                }
            };
            var mockRepo = RepositoryUtils.MockRepository<Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var mockPinger = new Mock<IPinger>();
            mockPinger.Setup(p => p.PingAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((string url, int timeout) =>
                   {
                       return new PingResult((url == "localhost2"), 0);
                   });

            var validator = new NullValidator<RefreshServicesStatus>();

            var command = new RefreshServicesStatus(10);
            var sut = new RefreshServicesStatusHandler(mockDbContext.Object, mockPinger.Object, validator);
            await sut.Handle(command);

            foreach(var endpoint in service.Endpoints)
                mockPinger.Verify(m => m.PingAsync(endpoint.Url, command.Timeout), Times.Once());

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Infrastructure.Entities.Service, bool>>>(),
                                                  It.Is<Infrastructure.Entities.Service>(r => r.Active == true && 1 == r.Endpoints.Count(es => es.Active && es.Url == "localhost2" ) ) ),
                            Times.Once());
        }

        [Fact]
        public async Task should_update_endpoint_roundtrip_time()
        {
            var service = new Infrastructure.Entities.Service()
            {
                Active = false,
                Name = "lorem",
                Endpoints = new[]{
                    new Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = false,
                        Url = "localhost",
                        RoundtripTime = long.MaxValue
                    }
                }
            };
            var mockRepo = RepositoryUtils.MockRepository<Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var mockPinger = new Mock<IPinger>();
            mockPinger.Setup(p => p.PingAsync(It.IsAny<string>(), It.IsAny<int>()))
                      .ReturnsAsync(new PingResult(true, 42));

            var validator = new NullValidator<RefreshServicesStatus>();

            var command = new RefreshServicesStatus(10);
            var sut = new RefreshServicesStatusHandler(mockDbContext.Object, mockPinger.Object, validator);
            await sut.Handle(command);

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Infrastructure.Entities.Service, bool>>>(),
                                                  It.Is<Infrastructure.Entities.Service>(r => r.Endpoints.Any(es => es.RoundtripTime == 42))),
                            Times.Once());
        }
    }
}
