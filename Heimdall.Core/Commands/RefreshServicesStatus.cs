using MediatR;
using System;


namespace Heimdall.Core.Commands
{
    public class RefreshServicesStatus : INotification
    {
        public RefreshServicesStatus(int timeout)
        {
            this.Timeout = timeout;
        }

        public int Timeout { get; private set; }
    }
}
