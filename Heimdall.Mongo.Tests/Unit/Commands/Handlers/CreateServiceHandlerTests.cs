﻿using Heimdall.Core.Commands;
using Heimdall.Mongo.Commands.Handlers;
using Heimdall.Mongo.Infrastructure;
using Heimdall.Mongo.Tests.Common.Utils;
using LibCore.CQRS.Validation;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Heimdall.Mongo.Tests.Unit.Commands.Handlers
{
    public class CreateServiceHandlerTests
    {
        [Fact]
        public void should_throw_ArgumentNullException_when_dbContext_null()
        {
            var mockValidator = new Mock<IValidator<CreateService>>();
            Assert.Throws<ArgumentNullException>(() => new CreateServiceHandler(null, mockValidator.Object));
        }

        [Fact]
        public async Task should_fail_when_command_null()
        {
            var mockDbContext = new Mock<IDbContext>();
            var mockValidator = new Mock<IValidator<CreateService>>();

            var sut = new CreateServiceHandler(mockDbContext.Object, mockValidator.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(null));
        }

        [Fact]
        public async Task should_insert_service()
        {
            var command = new CreateService(Guid.NewGuid(), "lorem");

            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<CreateService>();

            var sut = new CreateServiceHandler(mockDbContext.Object, validator);
            await sut.Handle(command);

            mockRepo.Verify(m => m.InsertOneAsync(It.Is<Mongo.Infrastructure.Entities.Service>(r =>
                    r.Id == command.ServiceId &&
                    r.Name == command.ServiceName &&
                    r.Active == false &&
                    null != r.Endpoints && 0 == r.Endpoints.Count()) 
                ), Times.Once());
        }
    }
}
