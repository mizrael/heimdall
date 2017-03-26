using System;

namespace Heimdall.Web.DTO
{
    public class ServiceArchiveItem
    {
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public int EndpointsCount { get; set; } = 0;
        public long RoundtripTime { get; set; } = long.MaxValue;
    }
}
