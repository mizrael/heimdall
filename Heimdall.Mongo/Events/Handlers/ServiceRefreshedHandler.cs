using Heimdall.Core.Events;
using Heimdall.Mongo.Infrastructure;
using MediatR;
using System;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Events.Handlers
{
    public class ServiceRefreshedHandler : IAsyncNotificationHandler<ServiceRefreshed>
    {
        private readonly IDbContext _db;

        public ServiceRefreshedHandler(IDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }
        
        public async Task Handle(ServiceRefreshed @event)
        {
            if (null == @event)
                throw new ArgumentNullException(nameof(@event));

            var service = await _db.Services.FindOneAsync(s => s.Id == @event.ServiceId);
            if (null == service)
                throw new ArgumentOutOfRangeException(nameof(@event), "invalid service id");

            await _db.TraceEvents.InsertOneAsync(new Infrastructure.Entities.TraceEvent()
            {
                Id = Guid.NewGuid(),
                CreationDate = DateTime.UtcNow.Ticks,
                Name = Core.TraceEventNames.ServiceRefreshed,
                CustomData = service
            });
        }
    }
}
