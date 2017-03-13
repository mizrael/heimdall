using Heimdall.Core.Queries.Models;
using MediatR;
using System;

namespace Heimdall.Core.Queries
{
    public class FindService : IRequest<Service>
    {
        public FindService(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName));
            this.ServiceName = serviceName;
        }
        public string ServiceName { get; private set; }
    }
}
