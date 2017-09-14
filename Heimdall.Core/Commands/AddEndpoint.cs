﻿using MediatR;
using System;

namespace Heimdall.Core.Commands
{
    public class AddEndpoint : INotification
    {
        public AddEndpoint(Guid id, string serviceName, string protocol, string address)
        {
            if (Guid.Empty.Equals(id))
                throw new ArgumentOutOfRangeException(nameof(id));
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName));
            if (string.IsNullOrWhiteSpace(protocol))
                throw new ArgumentNullException(nameof(protocol));
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException(nameof(address));

            this.EndpointId = id;
            this.ServiceName = serviceName;
            this.Address = address;
            this.Protocol = protocol;
        }

        public Guid EndpointId { get; private set; }
        public string ServiceName { get; private set; }
        public string Address { get; private set; }
        public string Protocol { get; private set; }
    }
}
