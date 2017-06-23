using Heimdall.Analytics.Mongo.Infrastructure;
using Heimdall.Core.Events;
using Heimdall.Mongo.Infrastructure;
using MediatR;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace Heimdall.Analytics.Mongo.Events.Handlers
{
    public class ServiceRefreshedHandler : IAsyncNotificationHandler<ServiceRefreshed>
    {
        private readonly IAnalyticsDbContext _analyticsDb;
        private readonly IDbContext _db;

        public ServiceRefreshedHandler(IDbContext db, IAnalyticsDbContext analyticsDb)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _analyticsDb = analyticsDb ?? throw new ArgumentNullException(nameof(analyticsDb));
        }

        public async Task Handle(ServiceRefreshed @event)
        {
            if (null == @event)
                throw new ArgumentNullException(nameof(@event));

            var service = await _db.Services.FindOneAsync(s => s.Id == @event.ServiceId);
            if (null == service)
                throw new ArgumentOutOfRangeException(nameof(@event), "invalid service id");

            var now = DateTime.UtcNow;

            var healthDetails = new Infrastructure.Entities.ServiceHealthDetails()
            {
                Timestamp = now.Ticks,
                RoundtripTime = long.MaxValue,
                BestEndpoint = string.Empty
            };

            var bestEndpoint = service.FindBestEndpoint();
            if (null != bestEndpoint)
            {
                healthDetails.RoundtripTime = bestEndpoint.RoundtripTime;
                healthDetails.BestEndpoint = bestEndpoint.Url;
            }

            var update = Builders<Infrastructure.Entities.ServiceHealth>.Update.Push(sh => sh.Details, healthDetails);

            var start = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, DateTimeKind.Utc);
            
            var serviceHealth = await _analyticsDb.ServicesHealth.FindOneAndUpdateAsync(sh => sh.ServiceId == service.Id && sh.TimestampMinute == start.Ticks, update);
            if (null != serviceHealth)
                return;

            var newServiceHealth = new Infrastructure.Entities.ServiceHealth()
            {
                ServiceId = service.Id,
                TimestampMinute = start.Ticks,
                Details = new []{ healthDetails }
            };
            await _analyticsDb.ServicesHealth.InsertOneAsync(newServiceHealth);
        }
    }
}
