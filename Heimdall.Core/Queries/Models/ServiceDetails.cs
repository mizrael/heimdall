using System.Collections.Generic;

namespace Heimdall.Core.Queries.Models
{
    public class ServiceDetails
    {
        public string Name { get; set; }
        public bool Active { get; set; } = false;
        public ServiceEndpoint BestEndpoint { get; set; }
        public IEnumerable<ServiceEndpoint> Endpoints { get; set; }
    }

    public class ServiceEndpoint
    {
        public string Address { get; set; }
        public string Protocol { get; set; }
        public bool Active { get; set; }
        public long RoundtripTime { get; set; }
    }
}
