using System.Collections.Generic;

namespace Heimdall.API.Models
{
    public class Service
    {
        public string Name { get; set; }
        public IEnumerable<ServiceEndpoints> Endpoints { get; set; }
    }

    public class ServiceEndpoints
    {
        public string Url { get; set; }
        public long RoundtripTime { get; set; }
    }
}
