using System;

namespace Heimdall.Mongo.Infrastructure.Entities
{
    public class TraceEvent
    {
        public Guid Id { get; set; }
        public long CreationDate { get; set; } = DateTime.UtcNow.Ticks;
        public string Name { get; set; } = string.Empty;
        public object CustomData { get; set; }
    }
}
