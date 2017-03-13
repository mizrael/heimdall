using Heimdall.Core.Commands;
using Heimdall.Mongo.Commands.Handlers;
using Heimdall.Mongo.Infrastructure;
using Heimdall.Mongo.Tests.Utils;
using LibCore.CQRS.Validation;
using LibCore.Mongo;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Heimdall.Mongo.Tests.Unit.Commands.Handlers
{
    public class UpsertServiceHandlerTests
    {
        [Fact]
        public void should_throw_ArgumentNullException_when_dbContext_null()
        {
            var mockValidator = new Mock<IValidator<UpsertService>>();
            Assert.Throws<ArgumentNullException>(() => new UpsertServiceHandler(null, mockValidator.Object));
        }

        [Fact]
        public async Task should_fail_when_command_null()
        {
            var mockDbContext = new Mock<IDbContext>();
            var mockValidator = new Mock<IValidator<UpsertService>>();

            var sut = new UpsertServiceHandler(mockDbContext.Object, mockValidator.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(null));
        }

        [Fact]
        public async Task should_insert_service_when_not_found()
        {
            var command = new UpsertService("lorem", "ipsum");

            var mockRepo = RepositoryUtils.MockRepository<Infrastructure.Entities.Service>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<UpsertService>();

            var sut = new UpsertServiceHandler(mockDbContext.Object, validator);
            await sut.Handle(command);

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Infrastructure.Entities.Service, bool>>>(),
                It.Is<Infrastructure.Entities.Service>(r =>
                    r.Name == command.Name &&
                    null != r.Endpoints && 1 == r.Endpoints.Count() && 
                    r.Endpoints.Any(es => es.Active == true && es.Url == command.Endpoint)) 
                ), Times.Once());
        }

        [Fact]
        public async Task should_update_service_when_found_with_no_endpoints()
        {
            var command = new UpsertService("lorem", "ipsum");

            var service = new Infrastructure.Entities.Service()
            {
                Name = command.Name,
                Endpoints = null
            };

            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<UpsertService>();

            var sut = new UpsertServiceHandler(mockDbContext.Object, validator);
            await sut.Handle(command);
            
            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Infrastructure.Entities.Service, bool>>>(),
                It.Is<Infrastructure.Entities.Service>(r =>
                    r.Name == command.Name &&
                    null != r.Endpoints && 1 == r.Endpoints.Count() &&
                    r.Endpoints.Any(es => es.Active == true && es.Url == command.Endpoint))
                ), Times.Once());
        }

        [Fact]
        public async Task should_update_service_when_found_with_other_endpoints()
        {
            var command = new UpsertService("lorem", "ipsum");

            var service = new Infrastructure.Entities.Service()
            {
                Name = command.Name,
                Endpoints = new[]
                {
                    new Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = false,
                        Url = "localhost"
                    }
                }
            };

            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<UpsertService>();

            var sut = new UpsertServiceHandler(mockDbContext.Object, validator);
            await sut.Handle(command);

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Infrastructure.Entities.Service, bool>>>(),
                It.Is<Infrastructure.Entities.Service>(r =>
                    r.Name == command.Name &&
                    null != r.Endpoints && 2 == r.Endpoints.Count() &&
                    r.Endpoints.Any(es => es.Active == true && es.Url == command.Endpoint))
                ), Times.Once());
        }
    }
}
