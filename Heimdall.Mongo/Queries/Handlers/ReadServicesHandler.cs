using Heimdall.Mongo.Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Queries.Handlers
{
    public class ReadServicesHandler : IAsyncRequestHandler<Core.Queries.ReadServices, IEnumerable<Core.Queries.Models.ServiceArchiveItem>>
    {
        private IDbContext _db;

        public ReadServicesHandler(IDbContext db) 
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<IEnumerable<Core.Queries.Models.ServiceArchiveItem>> Handle(Core.Queries.ReadServices query)
        {
            if (null == query)
                throw new ArgumentNullException(nameof(query));

            var services = await _db.Services.FindAsync(s => true);
            if (null == services)
                return Enumerable.Empty<Core.Queries.Models.ServiceArchiveItem>();
            
            return services.Select(s => AutoMapper.Mapper.Map<Core.Queries.Models.ServiceArchiveItem>(s))
                           .ToArray();
        }
    }
}
