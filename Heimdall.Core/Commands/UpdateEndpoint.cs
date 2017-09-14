using MediatR;
using System;

namespace Heimdall.Core.Commands
{
    public class UpdateEndpoint : INotification
    {
        public UpdateEndpoint(Guid endpointId, string serviceName, string protocol, string address)
        {
            if (Guid.Empty.Equals(endpointId))
                throw new ArgumentOutOfRangeException(nameof(endpointId));
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName));
            if (string.IsNullOrWhiteSpace(protocol))
                throw new ArgumentNullException(nameof(protocol));
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException(nameof(address));

            this.EndpointId = endpointId;
            this.ServiceName = serviceName;
            this.Protocol = protocol;
            this.Address = address;
        }

        public Guid EndpointId { get; private set; }
        public string ServiceName { get; private set; }
        public string Protocol { get; private set; }
        public string Address { get; private set; }

        
    }
}