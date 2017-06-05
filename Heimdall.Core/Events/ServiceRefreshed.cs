using System;
using MediatR;

namespace Heimdall.Core.Events
{
    public class ServiceRefreshed : INotification
    {
        public ServiceRefreshed(Guid serviceId)
        {
            if (serviceId == Guid.Empty)
                throw new ArgumentOutOfRangeException(nameof(serviceId));
            this.ServiceId = serviceId;
        }
        public Guid ServiceId { get; private set; }
    }
}
