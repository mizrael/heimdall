using Heimdall.Core.Queries.Models;
using MediatR;
using System.Collections.Generic;

namespace Heimdall.Core.Queries
{
    public class ReadServices : IRequest<IEnumerable<ServiceArchiveItem>>
    {
        
    }
}
