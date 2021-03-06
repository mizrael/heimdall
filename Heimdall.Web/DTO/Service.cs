﻿using System.Collections.Generic;

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
        public bool Active { get; set; } = false;
        public ServiceEndpoint BestEndpoint { get; set; }
        public IEnumerable<ServiceEndpoint> Endpoints { get; set; }
    }

    public class ServiceEndpoint
    {
        public System.Guid Id { get; set; }
        public string Address { get; set; }
        public string Protocol { get; set; }
        public bool Active { get; set; }
        public long RoundtripTime { get; set; }
    }

    public class CreateService
    {
        public string Name { get; set; }
    }

    public class AddEndpoint
    {
        public string ServiceName { get; set; }
        public string Address { get; set; }
        public string Protocol { get; set; }
    }

    public class UpdateEndpoint
    {
        public System.Guid EndpointId { get; set; }
        public string ServiceName { get; set; }
        public string Address { get; set; }
        public string Protocol { get; set; }
    }

    public class RemoveEndpoint
    {
        public string ServiceName { get; set; }
        public string Address { get; set; }
        public string Protocol { get; set; }
    }
}
