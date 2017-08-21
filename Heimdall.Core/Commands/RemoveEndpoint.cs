using MediatR;
using System;

namespace Heimdall.Core.Commands
{
    public class RemoveEndpoint : INotification
    {
        public RemoveEndpoint(string serviceName, string protocol, string address)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName));
            if (string.IsNullOrWhiteSpace(protocol))
                throw new ArgumentNullException(nameof(protocol));
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException(nameof(address));

            this.ServiceName = serviceName;
            this.Address = address;
            this.Protocol = protocol;
        }

        public string ServiceName { get; private set; }
        public string Address { get; private set; }
        public string Protocol { get; private set; }
    }
}
