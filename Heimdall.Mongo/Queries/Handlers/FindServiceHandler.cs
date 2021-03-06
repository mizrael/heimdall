﻿using Heimdall.Mongo.Infrastructure;
using MediatR;
using System;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Queries.Handlers
{
    public class FindServiceHandler : IAsyncRequestHandler<Core.Queries.FindService, Core.Queries.Models.ServiceDetails>
    {
        private IDbContext _db;

        public FindServiceHandler(IDbContext db) 
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<Core.Queries.Models.ServiceDetails> Handle(Core.Queries.FindService query)
        {
            if (null == query)
                throw new ArgumentNullException(nameof(query));

            var service = await _db.Services.FindOneAsync(s => s.Id == query.ServiceId);
            if (null == service)
                return null;

            return AutoMapper.Mapper.Map<Core.Queries.Models.ServiceDetails>(service);
        }
    }
}
