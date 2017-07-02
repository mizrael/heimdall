using Heimdall.Analytics.Mongo.Infrastructure;
using Heimdall.Analytics.Queries;
using Heimdall.Mongo.Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Heimdall.Analytics.Queries.Models;
using System.Linq;

namespace Heimdall.Analytics.Mongo.Queries.Handlers
{
    public class ReadServiceHealthHandler : IAsyncRequestHandler<ReadServiceHealth, Analytics.Queries.Models.ServiceHealth>
    {
        private readonly IAnalyticsDbContext _analyticsDb;
        private readonly IDbContext _db;

        public ReadServiceHealthHandler(IDbContext db, IAnalyticsDbContext analyticsDb)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _analyticsDb = analyticsDb ?? throw new ArgumentNullException(nameof(analyticsDb));
        }

        public async Task<ServiceHealth> Handle(ReadServiceHealth query)
        {
            if (null == query)
                throw new ArgumentNullException(nameof(query));

            var service = await _db.Services.FindOneAsync(s => s.Name == query.ServiceName);
            if (null == service)
                throw new ArgumentException($"Invalid service name: '{query.ServiceName}'");

            var result = new ServiceHealth()
            {
                ServiceName = service.Name,
                From = query.From,
                To = query.To
            };

            var healthData = await _analyticsDb.ServicesHealth.FindAsync(sh => sh.ServiceId == service.Id &&
                                                                               sh.TimestampMinute >= query.From.Ticks && 
                                                                               sh.TimestampMinute < query.To.Ticks);
            if (null == healthData || !healthData.Any())
                return result;

            result.Details = healthData.SelectMany(h => h.Details).Select(hd => new ServiceHealthDetails()
            {
                When = new DateTime(hd.Timestamp),
                BestEndpoint = hd.BestEndpoint,
                RoundtripTime = hd.RoundtripTime
            }).ToArray();

            return result;
        }
    }
    
}
