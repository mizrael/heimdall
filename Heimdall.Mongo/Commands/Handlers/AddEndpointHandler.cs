﻿using Heimdall.Core.Commands;
using LibCore.CQRS.Commands.Handlers;
using System;
using System.Linq;
using LibCore.CQRS.Validation;
using Heimdall.Mongo.Infrastructure;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Commands.Handlers
{
    public class AddEndpointHandler : BaseCommandHandler<AddEndpoint>
    {
        private IDbContext _db;

        public AddEndpointHandler(IDbContext db, IValidator<AddEndpoint> validator) : base(validator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected override async Task RunCommand(AddEndpoint command)
        {
            var service = await _db.Services.FindOneAsync(s => s.Id == command.ServiceId);
            
            service.Endpoints = service.Endpoints ?? Enumerable.Empty<Infrastructure.Entities.ServiceEndpoint>();

            if (service.Endpoints.Any(e => e.Id == command.EndpointId))
                return;

            service.Endpoints = service.Endpoints.Append(new Infrastructure.Entities.ServiceEndpoint()
            {
                Id = command.EndpointId,
                Active = false,
                Address = command.Address,
                Protocol = command.Protocol
            });
            await _db.Services.UpsertOneAsync(s => s.Id == command.ServiceId, service);
        }
    }
}
