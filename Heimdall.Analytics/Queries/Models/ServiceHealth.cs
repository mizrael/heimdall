using System;
using System.Collections.Generic;

namespace Heimdall.Analytics.Queries.Models
{
    public class ServiceHealth
    {
        public string ServiceName { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public IEnumerable<ServiceHealthDetails> Details { get; set; }
    }

    public class ServiceHealthDetails
    {
        public DateTime When { get; set; }

        public long RoundtripTime { get; set; }
        
        public string BestEndpoint { get; set; }
    }
}
