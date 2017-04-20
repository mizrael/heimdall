using MediatR;
using System;

namespace Heimdall.Core.Commands
{
    public class RemoveEndpoint : INotification
    {
        public RemoveEndpoint(string name, string endpoint)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentNullException(nameof(endpoint));
            
            this.Name = name;
            this.Endpoint = endpoint;
        }
        
        public string Name{ get; private set; }
        public string Endpoint { get; private set; }
    }
}
