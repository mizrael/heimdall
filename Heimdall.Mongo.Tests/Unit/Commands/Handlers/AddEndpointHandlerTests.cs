using Heimdall.Core.Commands;
using Heimdall.Mongo.Commands.Handlers;
using Heimdall.Mongo.Infrastructure;
using Heimdall.Mongo.Tests.Common.Utils;
using LibCore.CQRS.Validation;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Heimdall.Mongo.Tests.Unit.Commands.Handlers
{
    public class AddEndpointHandlerTests
    {
        [Fact]
        public void should_throw_ArgumentNullException_when_dbContext_null()
        {
            var mockValidator = new Mock<IValidator<AddEndpoint>>();
            Assert.Throws<ArgumentNullException>(() => new AddEndpointHandler(null, mockValidator.Object));
        }

        [Fact]
        public async Task should_fail_when_command_null()
        {
            var mockDbContext = new Mock<IDbContext>();
            var mockValidator = new Mock<IValidator<AddEndpoint>>();

            var sut = new AddEndpointHandler(mockDbContext.Object, mockValidator.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(null));
        }

        [Fact]
        public async Task should_throw_when_service_not_found()
        {
            var command = new AddEndpoint(Guid.NewGuid(), Guid.NewGuid(), "ipsum", "dolor");

            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<AddEndpoint>();

            var sut = new AddEndpointHandler(mockDbContext.Object, validator);
            await Assert.ThrowsAsync<NullReferenceException>(() => sut.Handle(command) );
        }

        [Fact]
        public async Task should_update_service_when_found_with_no_endpoints()
        {
            var command = new AddEndpoint(Guid.NewGuid(), Guid.NewGuid(), "ipsum", "dolor");

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = command.ServiceId,
                Name = "lorem",
                Active = false,
                Endpoints = null
            };

            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<AddEndpoint>();

            var sut = new AddEndpointHandler(mockDbContext.Object, validator);
            await sut.Handle(command);
            
            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Mongo.Infrastructure.Entities.Service, bool>>>(),
                It.Is<Mongo.Infrastructure.Entities.Service>(r =>
                    r.Id == command.ServiceId &&
                    r.Active == false && 
                    null != r.Endpoints && 1 == r.Endpoints.Count() &&
                    r.Endpoints.Any(es => es.Active == false && 
                                            es.Id == command.EndpointId && 
                                            es.Address == command.Address && 
                                            es.Protocol == command.Protocol))
                ), Times.Once());
        }

        [Fact]
        public async Task should_update_service_when_found_with_other_protocol()
        {
            var command = new AddEndpoint(Guid.NewGuid(), Guid.NewGuid(), "ipsum", "dolor");

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = command.ServiceId,
                Name = "lorem",
                Active = false,
                Endpoints = new[]
                {
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Id = Guid.NewGuid(),
                        Active = false,
                        Address = command.Address,
                        Protocol = "amet"
                    }
                }
            };

            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<AddEndpoint>();

            var sut = new AddEndpointHandler(mockDbContext.Object, validator);
            await sut.Handle(command);

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Mongo.Infrastructure.Entities.Service, bool>>>(), 
                It.Is<Mongo.Infrastructure.Entities.Service>(r =>
                    r.Id == command.ServiceId &&
                    r.Active == false &&
                    null != r.Endpoints && 2 == r.Endpoints.Count() &&
                    r.Endpoints.Any(es => es.Active == false &&
                                            es.Id == command.EndpointId &&
                                            es.Address == command.Address && 
                                            es.Protocol == command.Protocol))
                ), Times.Once());
        }

        [Fact]
        public async Task should_update_service_when_found_with_other_endpoints()
        {
            var command = new AddEndpoint(Guid.NewGuid(), Guid.NewGuid(), "ipsum", "dolor");

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = command.ServiceId,
                Name = "lorem",
                Active = false,
                Endpoints = new[]
                {
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Id = Guid.NewGuid(),
                        Active = false,
                        Address = "localhost",
                        Protocol = "ipsum"
                    }
                }
            };

            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<AddEndpoint>();

            var sut = new AddEndpointHandler(mockDbContext.Object, validator);
            await sut.Handle(command);

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Mongo.Infrastructure.Entities.Service, bool>>>(),
                It.Is<Mongo.Infrastructure.Entities.Service>(r =>
                    r.Id == command.ServiceId &&
                    r.Active == false &&
                    null != r.Endpoints && 2 == r.Endpoints.Count() &&
                    r.Endpoints.Any(es => es.Active == false &&
                                            es.Id == command.EndpointId &&
                                            es.Address == command.Address && 
                                            es.Protocol == command.Protocol))
                ), Times.Once());
        }

        [Fact]
        public async Task should_not_replace_endpoint_when_found_by_id()
        {
            var endpoint = new Mongo.Infrastructure.Entities.ServiceEndpoint()
            {
                Id = Guid.NewGuid(),
                Active = false,
                Address = "localhost",
                Protocol = "http"
            };

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Name = "lorem",
                Active = false,
                Endpoints = new[]
                {
                   endpoint
                }
            };

            var command = new AddEndpoint(service.Id, endpoint.Id, "ipsum", "dolor");

            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<AddEndpoint>();

            var sut = new AddEndpointHandler(mockDbContext.Object, validator);
            await sut.Handle(command);

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Mongo.Infrastructure.Entities.Service, bool>>>(),
                It.IsAny<Mongo.Infrastructure.Entities.Service>()), Times.Never());
        }
    }
}
