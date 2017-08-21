﻿using Heimdall.Core.Commands;
using LibCore.CQRS.Commands.Handlers;
using System;
using System.Linq;
using LibCore.CQRS.Validation;
using Heimdall.Mongo.Infrastructure;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Commands.Handlers
{
    public class RemoveEndpointHandler : BaseCommandHandler<RemoveEndpoint>
    {
        private IDbContext _db;

        public RemoveEndpointHandler(IDbContext db, IValidator<RemoveEndpoint> validator) : base(validator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected override async Task RunCommand(RemoveEndpoint command)
        {
            var service = await _db.Services.FindOneAsync(s => s.Name == command.ServiceName);

            service.Endpoints = service.Endpoints.Where(e => e.Address != command.Address || e.Protocol != command.Protocol).ToArray();

            await _db.Services.UpsertOneAsync(s => s.Name == command.ServiceName, service);
        }
    }
}
