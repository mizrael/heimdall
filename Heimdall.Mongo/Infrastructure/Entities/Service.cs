using System;
using System.Collections.Generic;

namespace Heimdall.Mongo.Infrastructure.Entities
{
    public class Service
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public IEnumerable<ServiceEndpoint> Endpoints { get; set; }
    }

    public class ServiceEndpoint
    {
        public string Url { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public long RoundtripTime { get; set; } = long.MaxValue;
    }
}
