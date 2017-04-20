using Heimdall.Core.Commands;
using Heimdall.Mongo.Commands.Handlers;
using Heimdall.Mongo.Infrastructure;
using Heimdall.Mongo.Tests.Utils;
using LibCore.CQRS.Validation;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Heimdall.Mongo.Tests.Unit.Commands.Handlers
{
    public class RemoveEndpointHandlerTests
    {
        [Fact]
        public void should_throw_ArgumentNullException_when_dbContext_null()
        {
            var mockValidator = new Mock<IValidator<RemoveEndpoint>>();
            Assert.Throws<ArgumentNullException>(() => new RemoveEndpointHandler(null, mockValidator.Object));
        }

        [Fact]
        public async Task should_fail_when_command_null()
        {
            var mockDbContext = new Mock<IDbContext>();
            var mockValidator = new Mock<IValidator<RemoveEndpoint>>();

            var sut = new RemoveEndpointHandler(mockDbContext.Object, mockValidator.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(null));
        }

        [Fact]
        public async Task should_throw_when_service_not_found()
        {
            var command = new RemoveEndpoint("lorem", "ipsum");

            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<RemoveEndpoint>();

            var sut = new RemoveEndpointHandler(mockDbContext.Object, validator);
            await Assert.ThrowsAsync<NullReferenceException>(() => sut.Handle(command) );
        }

        [Fact]
        public async Task should_throw_when_service_has_null_endpoints()
        {
            var command = new RemoveEndpoint("lorem", "ipsum");

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Name = command.Name,
                Active = false,
                Endpoints = null
            };

            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<RemoveEndpoint>();

            var sut = new RemoveEndpointHandler(mockDbContext.Object, validator);
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(command));
        }
        
        [Fact]
        public async Task should_remove_endpoint()
        {
            var command = new RemoveEndpoint("lorem", "ipsum");

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Name = command.Name,
                Active = false,
                Endpoints = new[]
                {
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = false,
                        Url = command.Endpoint
                    }
                }
            };

            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<RemoveEndpoint>();

            var sut = new RemoveEndpointHandler(mockDbContext.Object, validator);
            await sut.Handle(command);

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Mongo.Infrastructure.Entities.Service, bool>>>(), 
                It.Is<Mongo.Infrastructure.Entities.Service>(r =>
                    r.Name == command.Name &&
                    r.Active == false &&
                    null != r.Endpoints && 0 == r.Endpoints.Count() )
                ), Times.Once());
        }

        [Fact]
        public async Task should_not_remove_endpoints_if_none_found()
        {
            var command = new RemoveEndpoint("lorem", "ipsum");

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Name = command.Name,
                Active = false,
                Endpoints = new[]
                {
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = false,
                        Url = "dolor"
                    }
                }
            };

            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<RemoveEndpoint>();

            var sut = new RemoveEndpointHandler(mockDbContext.Object, validator);
            await sut.Handle(command);

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Mongo.Infrastructure.Entities.Service, bool>>>(),
                It.Is<Mongo.Infrastructure.Entities.Service>(r =>
                    r.Name == command.Name &&
                    r.Active == false &&
                    null != r.Endpoints && 1 == r.Endpoints.Count() &&
                    r.Endpoints.ElementAt(0).Url == service.Endpoints.ElementAt(0).Url &&
                    r.Endpoints.ElementAt(0).Active == service.Endpoints.ElementAt(0).Active)
                ), Times.Once());
        }
    }
}
