﻿using Heimdall.Core.Commands;
using Heimdall.Mongo.Commands.Handlers;
using Heimdall.Mongo.Commands.Validation;
using Heimdall.Mongo.Infrastructure;
using Heimdall.Mongo.Tests.Utils;
using LibCore.CQRS.Validation;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace Heimdall.Mongo.Tests.Unit.Commands.Validation
{
    public class CreateServiceValidatorTests
    {
        [Fact]
        public async Task should_succeed_when_service_not_found()
        {
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var sut = new CreateServiceValidator(mockDbContext.Object);
            var result = await sut.ValidateAsync(new CreateService("lorem", "ipsum"));
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task should_fail_when_service_already_exists()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Active = true,
                Name = "lorem",
                Endpoints = Enumerable.Empty<Mongo.Infrastructure.Entities.ServiceEndpoint>()
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var sut = new CreateServiceValidator(mockDbContext.Object);
            var result = await sut.ValidateAsync(new CreateService(service.Name, "ipsum"));
            result.Success.Should().BeFalse();
            result.Errors.Any(e => e.Context == "service" && e.Message.Contains("already exists")).Should().BeTrue();
        }
    }
}