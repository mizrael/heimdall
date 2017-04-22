using MediatR;
using System;

namespace Heimdall.Core.Commands
{
    public class RemoveEndpoint : INotification
    {
        public RemoveEndpoint(string serviceName, string endpoint)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName));
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentNullException(nameof(endpoint));
            
            this.ServiceName = serviceName;
            this.Endpoint = endpoint;
        }
        
        public string ServiceName { get; private set; }
        public string Endpoint { get; private set; }
    }
}
