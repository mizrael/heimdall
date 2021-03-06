﻿using Heimdall.Core.Commands;
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
            var command = new RemoveEndpoint(Guid.NewGuid(), Guid.NewGuid());

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
            var command = new RemoveEndpoint(Guid.NewGuid(), Guid.NewGuid());

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = command.ServiceId,
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
            var command = new RemoveEndpoint(Guid.NewGuid(), Guid.NewGuid());

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = command.ServiceId,
                Active = false,
                Endpoints = new[]
                {
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Id = command.EndpointId,
                        Active = false,
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
                    r.Id == command.ServiceId &&
                    r.Active == false &&
                    null != r.Endpoints && !r.Endpoints.Any(e => e.Id == command.EndpointId) )
                ), Times.Once());
        }

        [Fact]
        public async Task should_not_remove_endpoints_if_none_found()
        {
            var endpoint1 = new Mongo.Infrastructure.Entities.ServiceEndpoint()
            {
                Id = Guid.NewGuid(),
                Active = false,
                Address = "dolor",
                Protocol = "ipsum"
            };
            var endpoint2 = new Mongo.Infrastructure.Entities.ServiceEndpoint()
            {
                Id = Guid.NewGuid(),
                Active = false,
                Address = "dolor",
                Protocol = "amet"
            };
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Name = "lorem",
                Active = false,
                Endpoints = new[]
                {
                    endpoint1, endpoint2
                }
            };

            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var validator = new NullValidator<RemoveEndpoint>();

            var sut = new RemoveEndpointHandler(mockDbContext.Object, validator);

            var command = new RemoveEndpoint(service.Id, Guid.NewGuid());
            await sut.Handle(command);

            mockRepo.Verify(m => m.UpsertOneAsync(It.IsAny<Expression<Func<Mongo.Infrastructure.Entities.Service, bool>>>(),
                It.Is<Mongo.Infrastructure.Entities.Service>(r =>
                    r.Id == service.Id &&
                    null != r.Endpoints && 2 == r.Endpoints.Count())
                ), Times.Once());
        }

    }
}
