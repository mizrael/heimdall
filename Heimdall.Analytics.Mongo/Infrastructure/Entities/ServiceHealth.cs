using System;
using System.Collections.Generic;

namespace Heimdall.Analytics.Mongo.Infrastructure.Entities
{
    public class ServiceHealth
    {
        public MongoDB.Bson.ObjectId Id { get; set; }

        public Guid ServiceId { get; set; }

        public long TimestampMinute { get; set; }

        /// <summary>
        /// contains the analytics data for the current minute
        /// </summary>
        public IEnumerable<ServiceHealthDetails> Details { get; set; }
    }

    public class ServiceHealthDetails
    {
        public long RoundtripTime { get; set; }
        public long Timestamp { get; set; }
        public string BestEndpoint { get; set; }
    }
}
