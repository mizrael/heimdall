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
    public class BaseDbTests : IDisposable
    {
        private const string connStr = "mongodb://127.0.0.1:27018/heimdall_tests";

        private readonly DbFactory _dbFactory;

        public BaseDbTests()
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

    public class ServiceRefreshedHandlerTests : BaseDbTests
    { 
        [Fact, Trait("Category", "Integration")]
        public async Task should_create_service_health_data_when_not_found()
        {
            var endpoint = new Heimdall.Mongo.Infrastructure.Entities.ServiceEndpoint()
            {
                Active = true,
                CreationDate = DateTime.UtcNow.Ticks,
                RoundtripTime = 1,
                Address = "ipsum"
            };
            var service = new Heimdall.Mongo.Infrastructure.Entities.Service()
            {
                Active = true,
                CreationDate = DateTime.UtcNow.Ticks,
                Endpoints = new[]{ endpoint },
                Id = Guid.NewGuid(),
                Name = "lorem"
            };

            await base.DbContext.Services.InsertOneAsync(service);
            
            var sut = new ServiceRefreshedHandler(base.DbContext, base.AnalyticsDbContext);

            var @event = new ServiceRefreshed(service.Id);
            await sut.Handle(@event);

            var servicesHealth = await base.AnalyticsDbContext.ServicesHealth.FindAsync(sh => sh.ServiceId == service.Id);
            servicesHealth.Should().NotBeNullOrEmpty();
            servicesHealth.Count().ShouldBeEquivalentTo(1);

            var serviceHealth = servicesHealth.ElementAt(0);
            serviceHealth.Should().NotBeNull();
            serviceHealth.Details.Should().NotBeNullOrEmpty();
            serviceHealth.Details.Count().ShouldBeEquivalentTo(1);
            var details = serviceHealth.Details.ElementAt(0);
            details.BestEndpoint.ShouldBeEquivalentTo(endpoint.Address);
            details.RoundtripTime.ShouldBeEquivalentTo(endpoint.RoundtripTime);
        }
        
        [Fact, Trait("Category", "Integration")]
        public async Task should_append_service_health_data_when_found()
        {
            var endpoint = new Heimdall.Mongo.Infrastructure.Entities.ServiceEndpoint()
            {
                Active = true,
                CreationDate = DateTime.UtcNow.Ticks,
                RoundtripTime = 1,
                Address = "ipsum"
            };
            var service = new Heimdall.Mongo.Infrastructure.Entities.Service()
            {
                Active = true,
                CreationDate = DateTime.UtcNow.Ticks,
                Endpoints = new[] { endpoint },
                Id = Guid.NewGuid(),
                Name = "lorem"
            };

            await base.DbContext.Services.InsertOneAsync(service);

            var now = DateTime.UtcNow;
            var start = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, DateTimeKind.Utc);

            var serviceHealth = new Infrastructure.Entities.ServiceHealth()
            {
                ServiceId = service.Id,
                TimestampMinute = start.Ticks,
                Details = Enumerable.Empty<Infrastructure.Entities.ServiceHealthDetails>()
            };
            await base.AnalyticsDbContext.ServicesHealth.InsertOneAsync(serviceHealth);

            var sut = new ServiceRefreshedHandler(base.DbContext, base.AnalyticsDbContext);

            var @event = new ServiceRefreshed(service.Id);
            await sut.Handle(@event);

            var foundServicesHealth = await base.AnalyticsDbContext.ServicesHealth.FindAsync(sh => sh.ServiceId == service.Id);
            foundServicesHealth.Should().NotBeNullOrEmpty();
            foundServicesHealth.Count().ShouldBeEquivalentTo(1);

            var foundServiceHealth = foundServicesHealth.ElementAt(0);
            foundServiceHealth.Should().NotBeNull();
            foundServiceHealth.Details.Should().NotBeNullOrEmpty();
            foundServiceHealth.Details.Count().ShouldBeEquivalentTo(1);
            var foundDetails = foundServiceHealth.Details.ElementAt(0);
            foundDetails.BestEndpoint.ShouldBeEquivalentTo(endpoint.Address);
            foundDetails.RoundtripTime.ShouldBeEquivalentTo(endpoint.RoundtripTime);
        }

        [Fact, Trait("Category", "Integration")]
        public async Task should_create_service_health_data_when_found_with_older_timestamp()
        {
            var endpoint = new Heimdall.Mongo.Infrastructure.Entities.ServiceEndpoint()
            {
                Active = true,
                CreationDate = DateTime.UtcNow.Ticks,
                RoundtripTime = 1,
                Address = "ipsum"
            };
            var service = new Heimdall.Mongo.Infrastructure.Entities.Service()
            {
                Active = true,
                CreationDate = DateTime.UtcNow.Ticks,
                Endpoints = new[] { endpoint },
                Id = Guid.NewGuid(),
                Name = "lorem"
            };

            await base.DbContext.Services.InsertOneAsync(service);

            var now = DateTime.UtcNow;
            var start = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute - 1, 0, DateTimeKind.Utc);

            var serviceHealth = new Infrastructure.Entities.ServiceHealth()
            {
                ServiceId = service.Id,
                TimestampMinute = start.Ticks,
                Details = Enumerable.Empty<Infrastructure.Entities.ServiceHealthDetails>()
            };
            await base.AnalyticsDbContext.ServicesHealth.InsertOneAsync(serviceHealth);

            var sut = new ServiceRefreshedHandler(base.DbContext, base.AnalyticsDbContext);

            var @event = new ServiceRefreshed(service.Id);
            await sut.Handle(@event);

            var foundServicesHealth = await base.AnalyticsDbContext.ServicesHealth.FindAsync(sh => sh.ServiceId == service.Id);
            foundServicesHealth.Should().NotBeNullOrEmpty();
            foundServicesHealth.Count().ShouldBeEquivalentTo(2);
        }
    }
}
