﻿using Heimdall.Mongo.Infrastructure;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Queries.Handlers
{
    public class FindServiceHandler : IAsyncRequestHandler<Core.Queries.FindService, Core.Queries.Models.Service>
    {
        private IDbContext _db;

        public FindServiceHandler(IDbContext db) 
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<Core.Queries.Models.Service> Handle(Core.Queries.FindService query)
        {
            if (null == query)
                throw new ArgumentNullException(nameof(query));

            var service = await _db.Services.FindOneAsync(s => s.Active && 
                s.Name == query.ServiceName &&
                null != s.Endpoints &&
                s.Endpoints.Any());
            if (null == service)
                return null;

            var availableEndpoints = service.Endpoints.Where(es => es.Active).ToArray();
            if (!availableEndpoints.Any())
                return null;

            service.Endpoints = availableEndpoints;
            return AutoMapper.Mapper.Map<Core.Queries.Models.Service>(service);
        }
    }
}
