using Heimdall.Core.Commands;
using Heimdall.Mongo.Commands.Handlers;
using Heimdall.Mongo.Infrastructure;
using Heimdall.Mongo.Tests.Common.Utils;
using LibCore.CQRS.Validation;
using MediatR;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Heimdall.Mongo.Tests.Unit.Commands.Handlers
{
    public class RefreshServicesStatusHandlerTests
    {
        [Fact]
        public void should_throw_ArgumentNullException_when_dbContext_null()
        {
            var mockValidator = new Mock<IValidator<RefreshServicesStatus>>();
            var mockMediator = new Mock<IMediator>();
            Assert.Throws<ArgumentNullException>(() => new RefreshServicesStatusHandler(null, mockMediator.Object, mockValidator.Object));
        }

        [Fact]
        public void should_throw_ArgumentNullException_when_mediator_null()
        {
            var mockValidator = new Mock<IValidator<RefreshServicesStatus>>();
            var mockDbContext = new Mock<IDbContext>();
            Assert.Throws<ArgumentNullException>(() => new RefreshServicesStatusHandler(mockDbContext.Object, null, mockValidator.Object));
        }

        [Fact]
        public async Task should_fail_when_command_null()
        {
            var mockDbContext = new Mock<IDbContext>();
            var mockMediator = new Mock<IMediator>();
            var mockValidator = new Mock<IValidator<RefreshServicesStatus>>();

            var sut = new RefreshServicesStatusHandler(mockDbContext.Object, mockMediator.Object, mockValidator.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(null));
        }

        [Fact]
        public async Task should_publish_RefreshServiceStatus_command_for_each_service()
        {
            var services = new[]{
                new Mongo.Infrastructure.Entities.Service()
                {
                    Id = System.Guid.NewGuid(),
                    Active = true,
                    Name = "lorem",
                    Endpoints = Enumerable.Empty<Mongo.Infrastructure.Entities.ServiceEndpoint>()
                },
                new Mongo.Infrastructure.Entities.Service()
                {
                    Id = System.Guid.NewGuid(),
                    Active = true,
                    Name = "ipsum",
                    Endpoints = Enumerable.Empty<Mongo.Infrastructure.Entities.ServiceEndpoint>()
                }
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(services);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var mockMediator = new Mock<IMediator>();

            var validator = new NullValidator<RefreshServicesStatus>();

            var sut = new RefreshServicesStatusHandler(mockDbContext.Object, mockMediator.Object, validator);
            await sut.Handle(new RefreshServicesStatus(10));

            foreach(var service in services)
                mockMediator.Verify(m => m.Publish(It.Is<RefreshServiceStatus>(r => r.ServiceId == service.Id), It.IsAny<System.Threading.CancellationToken>()), Times.Once());
        }
    }
}
