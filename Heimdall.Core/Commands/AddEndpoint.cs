using MediatR;
using System;

namespace Heimdall.Core.Commands
{
    public class AddEndpoint : INotification
    {
        public AddEndpoint(Guid serviceId, Guid endpointId, string protocol, string address)
        {
            if (Guid.Empty.Equals(endpointId))
                throw new ArgumentOutOfRangeException(nameof(endpointId));
            if (Guid.Empty.Equals(serviceId))
                throw new ArgumentOutOfRangeException(nameof(serviceId));
            
            if (string.IsNullOrWhiteSpace(protocol))
                throw new ArgumentNullException(nameof(protocol));
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException(nameof(address));

            this.EndpointId = endpointId;
            this.ServiceId = serviceId;
            this.Address = address;
            this.Protocol = protocol;
        }

        public Guid EndpointId { get; private set; }
        public Guid ServiceId { get; private set; }
        public string Address { get; private set; }
        public string Protocol { get; private set; }
    }
}
