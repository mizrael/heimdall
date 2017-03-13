using Heimdall.Core.Queries;
using Heimdall.Mongo.Infrastructure;
using Heimdall.Mongo.Queries.Handlers;
using LibCore.Mongo;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Heimdall.Mongo.Tests.Utils;

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
            var mockRepo = RepositoryUtils.MockRepository<Infrastructure.Entities.Service>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var query = new FindService("lorem");
            
            var sut = new FindServiceHandler(mockDbContext.Object);
            var result = await sut.Handle(query);
            result.Should().BeNull();
        }

        [Fact]
        public async Task should_return_null_when_service_is_inactive()
        {
            var service = new Infrastructure.Entities.Service()
            {
                Name = "lorem",
                Active = false,
                Endpoints = new[]
                {
                    new Infrastructure.Entities.ServiceEndpoint(){Active = true, Url ="localhost"}
                }
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var query = new FindService("lorem");

            var sut = new FindServiceHandler(mockDbContext.Object);
            var result = await sut.Handle(query);
            result.Should().BeNull();
        }

        [Fact]
        public async Task should_return_null_when_service_has_no_endpoints()
        {
            var service = new Infrastructure.Entities.Service()
            {
                Name = "lorem",
                Endpoints = null
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var query = new FindService("lorem");

            var sut = new FindServiceHandler(mockDbContext.Object);
            var result = await sut.Handle(query);
            result.Should().BeNull();
        }

        [Fact]
        public async Task should_return_null_when_service_has_empty_endpoints_collection()
        {
            var service = new Infrastructure.Entities.Service()
            {
                Name = "lorem",
                Endpoints = Enumerable.Empty<Infrastructure.Entities.ServiceEndpoint>()
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var query = new FindService("lorem");

            var sut = new FindServiceHandler(mockDbContext.Object);
            var result = await sut.Handle(query);
            result.Should().BeNull();
        }

        [Fact]
        public async Task should_return_null_when_service_has_no_active_endpoints()
        {
            var service = new Infrastructure.Entities.Service()
            {
                Name = "lorem",
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

            var query = new FindService("lorem");

            var sut = new FindServiceHandler(mockDbContext.Object);
            var result = await sut.Handle(query);
            result.Should().BeNull();
        }

        [Fact]
        public async Task should_return_service_with_active_endpoints_only()
        {
            var service = new Infrastructure.Entities.Service()
            {
                Name = "lorem",
                Active = true,
                Endpoints = new[]
                {
                    new Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = false,
                        Url = "localhost1"
                    },
                    new Infrastructure.Entities.ServiceEndpoint()
                    {
                        Active = true,
                        Url = "localhost2"
                    }
                }
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var query = new FindService("lorem");

            var sut = new FindServiceHandler(mockDbContext.Object);
            var result = await sut.Handle(query);
            result.Should().NotBeNull();
            result.Endpoints.Should().NotBeNullOrEmpty();
            result.Endpoints.ElementAt(0).Url.ShouldBeEquivalentTo("localhost2");
        }

        
    }
}
