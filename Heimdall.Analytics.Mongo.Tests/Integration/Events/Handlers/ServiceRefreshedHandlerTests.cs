using Heimdall.Core.Events;
using Heimdall.Analytics.Mongo.Events.Handlers;
using Heimdall.Mongo.Infrastructure;
using Heimdall.Mongo.Tests.Common.Utils;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Heimdall.Analytics.Mongo.Infrastructure;
using System.Linq.Expressions;
using System.ComponentModel;
using LibCore.Mongo;
using FluentAssertions;

namespace Heimdall.Analytics.Mongo.Tests.Integration.Events.Handlers
{
    public class DatabaseFixture : IDisposable
    {
        private const string connStr = "mongodb://127.0.0.1:27018/heimdall_tests";

        private readonly DbFactory _dbFactory;

        public DatabaseFixture()
        {
            _dbFactory = new DbFactory();

            var repoFactory = new RepositoryFactory(_dbFactory);
            
            this.DbContext = new DbContext(repoFactory, connStr);

            this.AnalyticsDbContext = new AnalyticsDbContext(repoFactory, connStr);
        }

        public readonly DbContext DbContext;
        public readonly AnalyticsDbContext AnalyticsDbContext;

        public void Dispose()
        {
            var db = _dbFactory.GetDatabase(connStr);
            db.DropCollection(this.DbContext.Services.CollectionName);
            db.DropCollection(this.DbContext.TraceEvents.CollectionName);
            db.DropCollection(this.AnalyticsDbContext.ServicesHealth.CollectionName);
        }
    }

    public class ServiceRefreshedHandlerTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;

        public ServiceRefreshedHandlerTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact, Trait("Category", "Integration")]
        public async Task should_create_service_health_data_when_not_found()
        {
            var endpoint = new Heimdall.Mongo.Infrastructure.Entities.ServiceEndpoint()
            {
                Active = true,
                CreationDate = DateTime.UtcNow.Ticks,
                RoundtripTime = 1,
                Url = "ipsum"
            };
            var service = new Heimdall.Mongo.Infrastructure.Entities.Service()
            {
                Active = true,
                CreationDate = DateTime.UtcNow.Ticks,
                Endpoints = new[]{ endpoint },
                Id = Guid.NewGuid(),
                Name = "lorem"
            };

            await _fixture.DbContext.Services.InsertOneAsync(service);
            
            var sut = new ServiceRefreshedHandler(_fixture.DbContext, _fixture.AnalyticsDbContext);

            var @event = new ServiceRefreshed(service.Id);
            await sut.Handle(@event);

            var servicesHealth = await _fixture.AnalyticsDbContext.ServicesHealth.FindAsync(sh => sh.ServiceId == service.Id);
            servicesHealth.Should().NotBeNullOrEmpty();
            servicesHealth.Count().ShouldBeEquivalentTo(1);

            var serviceHealth = servicesHealth.ElementAt(0);
            serviceHealth.Should().NotBeNull();
            serviceHealth.Details.Should().NotBeNullOrEmpty();
            serviceHealth.Details.Count().ShouldBeEquivalentTo(1);
            var details = serviceHealth.Details.ElementAt(0);
            details.BestEndpoint.ShouldBeEquivalentTo(endpoint.Url);
            details.RoundtripTime.ShouldBeEquivalentTo(endpoint.RoundtripTime);
        }
    }
}
