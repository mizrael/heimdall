using MediatR;
using System;

namespace Heimdall.Core.Commands
{
    public class DeleteService : INotification
    {
        public DeleteService(Guid serviceId)
        {
            if (Guid.Empty == serviceId)
                throw new ArgumentOutOfRangeException(nameof(serviceId));

            this.ServiceId = serviceId;
        }

        public Guid ServiceId { get; private set; }
    }
}
