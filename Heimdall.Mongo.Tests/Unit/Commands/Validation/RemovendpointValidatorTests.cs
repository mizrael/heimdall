﻿using Heimdall.Core.Commands;
using Heimdall.Mongo.Infrastructure;
using Heimdall.Mongo.Tests.Common.Utils;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Heimdall.Mongo.Commands.Validation;

namespace Heimdall.Mongo.Tests.Unit.Commands.Validation
{
    public class RemovendpointValidatorTests
    {
        [Fact]
        public async Task should_succeed_when_command_valid()
        {
            var endpoint = new Mongo.Infrastructure.Entities.ServiceEndpoint()
            {
                Id = Guid.NewGuid(),
                Address = "ipsum",
                Protocol = "dolor",
                Active = false
            };

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = false,
                Name = "lorem",
                Endpoints = new[] { endpoint }
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var sut = new RemoveEndpointValidator(mockDbContext.Object);
            var result = await sut.ValidateAsync(new RemoveEndpoint(service.Id, endpoint.Id));
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task should_fail_when_endpoint_not_found()
        {
            var endpoint = new Mongo.Infrastructure.Entities.ServiceEndpoint()
            {
                Id = Guid.NewGuid(),
                Address = "ipsum",
                Protocol = "dolor",
                Active = false
            };
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = false,
                Name = "lorem",
                Endpoints = new[] { endpoint }
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var sut = new RemoveEndpointValidator(mockDbContext.Object);
            var command = new RemoveEndpoint(service.Id, Guid.NewGuid());
            var result = await sut.ValidateAsync(command);
            result.Success.Should().BeFalse();
            result.Errors.Any(e => e.Context == "endpoint" && e.Message.Contains(command.EndpointId.ToString())).Should().BeTrue();
        }

        [Fact]
        public async Task should_fail_when_service_not_found()
        {
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var sut = new RemoveEndpointValidator(mockDbContext.Object);
            var result = await sut.ValidateAsync(new RemoveEndpoint(Guid.NewGuid(), Guid.NewGuid()));
            result.Success.Should().BeFalse();
            result.Errors.Any(e => e.Context == "service" && e.Message.Contains("Unable to load service")).Should().BeTrue();
        }

        [Fact]
        public async Task should_fail_when_service_has_no_endpoints()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = false,
                Name = "lorem",
                Endpoints = null
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var sut = new RemoveEndpointValidator(mockDbContext.Object);
            var result = await sut.ValidateAsync(new RemoveEndpoint(service.Id, Guid.NewGuid()));
            result.Success.Should().BeFalse();
            result.Errors.Any(e => e.Context == "service" && e.Message.Contains("no endpoints")).Should().BeTrue();
        }
    }
}
