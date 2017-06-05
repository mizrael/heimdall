using System;
using System.Collections.Generic;
using System.Linq;

namespace Heimdall.Mongo.Infrastructure.Entities
{
    public class Service
    {
        public Guid Id { get; set; }
        public long CreationDate { get; set; } = DateTime.UtcNow.Ticks;
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public IEnumerable<ServiceEndpoint> Endpoints { get; set; }

        public IEnumerable<ServiceEndpoint> GetActiveEndpoints()
        {
            if (!this.Active || null == this.Endpoints)
                return Enumerable.Empty<ServiceEndpoint>();

            var availableEndpoints = this.Endpoints.Where(es => es.Active).ToArray();
            return availableEndpoints;
        }
    }

    public class ServiceEndpoint
    {
        public long CreationDate { get; set; } = DateTime.UtcNow.Ticks;
        public string Url { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public long RoundtripTime { get; set; } = long.MaxValue;
    }
}
