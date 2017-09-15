using MediatR;
using System;

namespace Heimdall.Core.Commands
{
    public class RemoveEndpoint : INotification
    {
        public RemoveEndpoint(Guid serviceId, Guid endpointId)
        {
            if (Guid.Empty.Equals(endpointId))
                throw new ArgumentOutOfRangeException(nameof(endpointId));
            if (Guid.Empty.Equals(serviceId))
                throw new ArgumentOutOfRangeException(nameof(serviceId));

            this.EndpointId = endpointId;
            this.ServiceId = serviceId;
        }

        public Guid EndpointId { get; private set; }
        public Guid ServiceId { get; private set; }
    }
}
