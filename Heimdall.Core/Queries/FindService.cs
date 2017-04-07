using Heimdall.Core.Queries.Models;
using MediatR;
using System;

namespace Heimdall.Core.Queries
{
    public class FindService : IRequest<ServiceDetails>
    {
        public FindService(string serviceName, bool forceLoad)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName));
            this.ServiceName = serviceName;
            this.ForceLoad = forceLoad;
        }
        public string ServiceName { get; private set; }
        public bool ForceLoad { get; private set; }
    }
}
