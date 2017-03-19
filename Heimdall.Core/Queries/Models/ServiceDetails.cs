using System;
using System.Collections.Generic;

namespace Heimdall.Core.Queries.Models
{
    public class ServiceDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<ServiceEndpoint> Endpoints { get; set; }
    }

    public class ServiceEndpoint
    {
        public string Url { get; set; }
        public bool Active { get; set; }
    }
}
