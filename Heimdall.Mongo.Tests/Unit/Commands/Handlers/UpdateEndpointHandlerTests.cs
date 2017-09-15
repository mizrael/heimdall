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
    public class UpdateEndpointHandlerTests
    {
        [Fact]
        public void should_throw_ArgumentNullException_when_dbContext_null()
        {
            var mockValidator = new Mock<IValidator<UpdateEndpoint>>();
            Assert.Throws<ArgumentNullException>(() => new UpdateEndpointHandler(null, mockValidator.Object));
        }

        [Fact]
        public async Task should_fail_when_command_null()
        {
            var mockDbContext = new Mock<IDbContext>();
            var mockValidator = new Mock<IValidator<UpdateEndpoint>>();

            var sut = new UpdateEndpointHandler(mockDbContext.Object, mockValidator.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(null));
        }

        [Fact]
        public async Task should_throw_when_service_not_found()
        {
            var command = new UpdateEndpoint(Guid.NewGuid(), Guid.NewGuid(), "lorem", "ipsum");

            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<UpdateEndpoint>();

            var sut = new UpdateEndpointHandler(mockDbContext.Object, validator);
            await Assert.ThrowsAsync<NullReferenceException>(() => sut.Handle(command));
        }

        [Fact]
        public async Task should_throw_when_endpoint_not_found()
        {
            var command = new UpdateEndpoint(Guid.NewGuid(), Guid.NewGuid(), "lorem", "ipsum");

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = command.ServiceId,
                Active = false,
                Endpoints = Enumerable.Empty<Mongo.Infrastructure.Entities.ServiceEndpoint>()
            };

            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<UpdateEndpoint>();

            var sut = new UpdateEndpointHandler(mockDbContext.Object, validator);
            await Assert.ThrowsAsync<NullReferenceException>(() => sut.Handle(command));
        }

        [Fact]
        public async Task should_update_endpoint_when_command_valid_and_deactivate_it()
        {
            var command = new UpdateEndpoint(Guid.NewGuid(), Guid.NewGuid(), "lorem", "ipsum");

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = command.ServiceId,
                Active = false,
                Endpoints = new[]
                {
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Id = command.EndpointId,
                        Active = true,
                        Address = "localhost",
                        Protocol = "http"
                    }
                }
            };

            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<UpdateEndpoint>();

            var sut = new UpdateEndpointHandler(mockDbContext.Object, validator);
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
    }
}
