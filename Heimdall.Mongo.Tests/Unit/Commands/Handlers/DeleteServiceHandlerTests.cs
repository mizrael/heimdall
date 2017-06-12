using Heimdall.Core.Commands;
using Heimdall.Mongo.Commands.Handlers;
using Heimdall.Mongo.Infrastructure;
using Heimdall.Mongo.Tests.Common.Utils;
using LibCore.CQRS.Validation;
using LibCore.Web.Services;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;


namespace Heimdall.Mongo.Tests.Unit.Commands.Handlers
{
    public class DeleteServiceHandlerTests
    {
        [Fact]
        public void should_throw_ArgumentNullException_when_dbContext_null()
        {
            var mockValidator = new Mock<IValidator<DeleteService>>();
            Assert.Throws<ArgumentNullException>(() => new DeleteServiceHandler(null,mockValidator.Object));
        }

        [Fact]
        public async Task should_fail_when_command_null()
        {
            var mockDbContext = new Mock<IDbContext>();
            var mockValidator = new Mock<IValidator<DeleteService>>();

            var sut = new DeleteServiceHandler(mockDbContext.Object, mockValidator.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(null));
        }

        [Fact]
        public async Task should_delete_servoce_when_input_valid()
        {
            var startTicks = DateTime.UtcNow.Ticks;

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = true,
                Name = "lorem",
                Endpoints = Enumerable.Empty<Mongo.Infrastructure.Entities.ServiceEndpoint>()
            };
            var mockServicesRepo = RepositoryUtils.MockRepository(service);
            
            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockServicesRepo.Object);

            var validator = new NullValidator<DeleteService>();

            var sut = new DeleteServiceHandler(mockDbContext.Object, validator);

            await sut.Handle(new DeleteService(service.Name));

            mockServicesRepo.Verify(m => m.DeleteOneAsync(It.IsAny<Expression<Func<Mongo.Infrastructure.Entities.Service, bool>>>()), Times.Once());
        }
    }
}
