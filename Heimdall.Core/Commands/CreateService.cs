using MediatR;
using System;

namespace Heimdall.Core.Commands
{
    public class CreateService : INotification
    {
        public CreateService(Guid serviceId, string serviceName)
        {
            if (Guid.Empty == serviceId)
                throw new ArgumentOutOfRangeException(nameof(serviceId));

            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName));
            
            this.ServiceName = serviceName;
            this.ServiceId = serviceId;
        }
        
        public string ServiceName { get; private set; }
        public Guid ServiceId { get; private set; }
    }
}
