﻿using System;
using System.Collections.Generic;

namespace Heimdall.Web.DTO
{
    public class ServiceArchiveItem
    {
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public int EndpointsCount { get; set; } = 0;
        public long RoundtripTime { get; set; } = long.MaxValue;
    }

    public class ServiceDetails
    {
        public string Name { get; set; }
        public IEnumerable<ServiceEndpoint> Endpoints { get; set; }
    }

    public class ServiceEndpoint
    {
        public string Url { get; set; }
        public bool Active { get; set; }
        public long RoundtripTime { get; set; }
    }
}
