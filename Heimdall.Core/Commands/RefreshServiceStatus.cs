using MediatR;
using System;

namespace Heimdall.Core.Commands
{
    public class RefreshServiceStatus : INotification
    {
        public RefreshServiceStatus(Guid serviceId, int timeout)
        {
            if (Guid.Empty == serviceId)
                throw new ArgumentOutOfRangeException(nameof(serviceId));
            this.ServiceId = serviceId;

            this.Timeout = timeout;
        }

        public Guid ServiceId { get; private set; }
        public int Timeout { get; private set; }
    }
}
