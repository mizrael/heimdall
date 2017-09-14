using Heimdall.Core.Commands;
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
            var result = await sut.ValidateAsync(new AddEndpoint(Guid.NewGuid(), service.Name, "ipsum", "dolor"));
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task should_succeed_when_endpoint_not_found()
        {
            var endpoint = new Mongo.Infrastructure.Entities.ServiceEndpoint()
            {
                Address = "ipsum",
                Protocol = "dolor",
                Active = false
            };

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Active = false,
                Name = "lorem",
                Endpoints = new[] { endpoint }
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var sut = new AddEndpointValidator(mockDbContext.Object);
            var result = await sut.ValidateAsync(new AddEndpoint(Guid.NewGuid(), service.Name, endpoint.Protocol, Guid.NewGuid().ToString() ));
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task should_fail_when_service_not_found()
        {
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var sut = new AddEndpointValidator(mockDbContext.Object);
            var result = await sut.ValidateAsync(new AddEndpoint(Guid.NewGuid(), "lorem", "ipsum", "dolor"));
            result.Success.Should().BeFalse();
            result.Errors.Any(e => e.Context == "service" && e.Message.Contains("Unable to load service")).Should().BeTrue();
        }

        [Fact]
        public async Task should_fail_when_endpoint_already_exists_by_address_and_protocol()
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
                Active = false,
                Name = "lorem",
                Endpoints = new[] { endpoint }
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var sut = new AddEndpointValidator(mockDbContext.Object);
            var result = await sut.ValidateAsync(new AddEndpoint(Guid.NewGuid(), service.Name, endpoint.Protocol, endpoint.Address));
            result.Success.Should().BeFalse();
            result.Errors.Any(e => e.Context == "endpoint" && e.Message.Contains(endpoint.Address)).Should().BeTrue();
        }
    
        [Fact]
        public async Task should_fail_when_endpoint_already_exists_by_id()
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
                Active = false,
                Name = "lorem",
                Endpoints = new[] { endpoint }
            };
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var sut = new AddEndpointValidator(mockDbContext.Object);
            var result = await sut.ValidateAsync(new AddEndpoint(endpoint.Id, service.Name, endpoint.Protocol, endpoint.Address));
            result.Success.Should().BeFalse();
            result.Errors.Any(e => e.Context == "endpoint" && e.Message.Contains(endpoint.Id.ToString())).Should().BeTrue();
        }
    }
}
