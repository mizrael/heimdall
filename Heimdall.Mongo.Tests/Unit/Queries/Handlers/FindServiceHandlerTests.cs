using Heimdall.Core.Queries;
using Heimdall.Mongo.Infrastructure;
using Heimdall.Mongo.Queries.Handlers;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Heimdall.Mongo.Tests.Common.Utils;

namespace Heimdall.Mongo.Tests.Unit.Queries.Handlers
{
    public class FindServiceHandlerTests
    {
        public FindServiceHandlerTests()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                Mongo.Infrastructure.MapperConfiguration.Register(cfg);
            });
        }

        [Fact]
        public void should_throw_ArgumentNullException_when_dbcontext_null()
        {
            Assert.Throws<ArgumentNullException>(() => new FindServiceHandler(null));
        }

        [Fact]
        public async Task should_throw_ArgumentNullException_when_query_null()
        {
            var mockDbContext = new Mock<IDbContext>();

            var sut = new FindServiceHandler(mockDbContext.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(null));
        }

        [Fact]
        public async Task should_return_null_when_service_not_existing()
        {
            var mockRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.Service>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var query = new FindService("lorem", false);
            
            var sut = new FindServiceHandler(mockDbContext.Object);
            var result = await sut.Handle(query);
            result.Should().BeNull();
        }

        [Fact]
        public async Task should_return_no_endpoints_when_service_is_inactive()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Name = "lorem",
                Active = false,
                Endpoints = new[]
                {
                    new Mongo.Infrastructure.Entities.ServiceEndpoint(){Active = true, Address ="localhost"}
                }
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var query = new FindService("lorem", false);

            var sut = new FindServiceHandler(mockDbContext.Object);
            var result = await sut.Handle(query);
            result.Should().NotBeNull();
            result.Name.ShouldBeEquivalentTo(service.Name);
            result.Endpoints.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task should_not_return_null_when_service_has_no_endpoints()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Name = "lorem",
                Endpoints = null
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var query = new FindService("lorem", false);

            var sut = new FindServiceHandler(mockDbContext.Object);
            var result = await sut.Handle(query);
            result.Should().NotBeNull();
            result.Name.ShouldBeEquivalentTo(service.Name);
            result.Endpoints.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task should_not_return_null_when_service_has_empty_endpoints_collection()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Name = "lorem",
                Endpoints = Enumerable.Empty<Mongo.Infrastructure.Entities.ServiceEndpoint>()
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var query = new FindService("lorem", false);

            var sut = new FindServiceHandler(mockDbContext.Object);
            var result = await sut.Handle(query);
            result.Should().NotBeNull();
            result.Name.ShouldBeEquivalentTo(service.Name);
            result.Endpoints.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task should_not_return_null_when_service_has_no_active_endpoints()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Name = "lorem",
                Endpoints = new[]
                {
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = false,
                        Address = "localhost"
                    }
                }
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var query = new FindService("lorem", false);

            var sut = new FindServiceHandler(mockDbContext.Object);
            var result = await sut.Handle(query);
            result.Should().NotBeNull();
            result.Name.ShouldBeEquivalentTo(service.Name);
            result.Endpoints.Should().BeNullOrEmpty();
        }

        [Fact]
        public async Task should_return_service_with_active_endpoints_only()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Name = "lorem",
                Active = true,
                Endpoints = new[]
                {
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = false,
                        Address = "localhost1"
                    },
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = true,
                        Address = "localhost2"
                    }
                }
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var query = new FindService("lorem", false);

            var sut = new FindServiceHandler(mockDbContext.Object);
            var result = await sut.Handle(query);
            result.Should().NotBeNull();
            result.Endpoints.Should().NotBeNullOrEmpty();
            result.Endpoints.ElementAt(0).Address.ShouldBeEquivalentTo("localhost2");
        }


        [Fact]
        public async Task should_return_service_with_all_endpoints_only_when_forceLoad_true()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Name = "lorem",
                Active = true,
                Endpoints = new[]
                {
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = false,
                        Address = "localhost1"
                    },
                    new Mongo.Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = true,
                        Address = "localhost2"
                    }
                }
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var query = new FindService("lorem", true);

            var sut = new FindServiceHandler(mockDbContext.Object);
            var result = await sut.Handle(query);
            result.Should().NotBeNull();
            result.Endpoints.Should().NotBeNullOrEmpty();
            result.Endpoints.Count().ShouldBeEquivalentTo(service.Endpoints.Count());
        }


    }
}
