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
using FluentAssertions;
using Heimdall.Mongo.Commands.Validation;

namespace Heimdall.Mongo.Tests.Unit.Commands.Validation
{
    public class AddEndpointValidatorTests
    {
        [Fact]
        public async Task should_succeed_when_command_valid()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Active = false,
                Name = "lorem",
                Endpoints = Enumerable.Empty<Mongo.Infrastructure.Entities.ServiceEndpoint>()
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var sut = new AddEndpointValidator(mockDbContext.Object);
            var result = await sut.ValidateAsync(new AddEndpoint(service.Name, "ipsum"));
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task should_succeed_when_endpoint_not_found()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Active = false,
                Name = "lorem",
                Endpoints = new[] { new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Url = "ipsum",
                        Active = false
                    }
                }
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var sut = new AddEndpointValidator(mockDbContext.Object);
            var result = await sut.ValidateAsync(new AddEndpoint(service.Name, "dolor"));
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task should_fail_when_service_not_found()
        {
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var sut = new AddEndpointValidator(mockDbContext.Object);
            var result = await sut.ValidateAsync(new AddEndpoint("lorem", "ipsum"));
            result.Success.Should().BeFalse();
            result.Errors.Any(e => e.Context == "service" && e.Message.Contains("Unable to load service")).Should().BeTrue();
        }

        [Fact]
        public async Task should_fail_when_endpoint_already_exists()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Active = false,
                Name = "lorem",
                Endpoints =new[] { new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Url = "ipsum",
                        Active = false
                    }
                }
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var sut = new AddEndpointValidator(mockDbContext.Object);
            var result = await sut.ValidateAsync(new AddEndpoint(service.Name, service.Endpoints.ElementAt(0).Url));
            result.Success.Should().BeFalse();
            result.Errors.Any(e => e.Context == "endpoint" && e.Message.Contains("already exists")).Should().BeTrue();
        }
    }
}
