using Heimdall.Core.Queries.Models;
using MediatR;
using System;

namespace Heimdall.Core.Queries
{
    public class FindService : IRequest<ServiceDetails>
    {
        public FindService(Guid serviceId)
        {
            this.ServiceId = serviceId;
        }
        public Guid ServiceId { get; private set; }
    }
}
