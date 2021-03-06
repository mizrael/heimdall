﻿using Heimdall.Core.Commands;
using Heimdall.Mongo.Infrastructure;
using LibCore.CQRS.Commands.Handlers;
using LibCore.CQRS.Validation;
using LibCore.Web.Services;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Commands.Handlers
{
    public class RefreshServiceStatusHandler : BaseCommandHandler<RefreshServiceStatus>
    {
        private readonly IDbContext _db;
        private readonly IPinger _pinger;
        private readonly IMediator _mediator;

        public RefreshServiceStatusHandler(IDbContext db, IPinger pinger, IMediator mediator, IValidator<RefreshServiceStatus> validator) : base(validator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _pinger = pinger ?? throw new ArgumentNullException(nameof(pinger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        protected override async Task RunCommand(RefreshServiceStatus command)
        {
            var service = await _db.Services.FindOneAsync(s => s.Id == command.ServiceId);
            if (null == service)
                return;

            service.Active = false;

            if (null != service.Endpoints && service.Endpoints.Any())
                foreach (var endpoint in service.Endpoints)
                {
                    try
                    {
                        var pingResult = await _pinger.PingAsync(endpoint.Address, command.Timeout);
                        service.Active |= pingResult.Success;
                        endpoint.Active = pingResult.Success;
                        endpoint.RoundtripTime = pingResult.RoundtripTime;
                    }
                    catch (Exception)
                    {
                        endpoint.Active = false;
                        endpoint.RoundtripTime = long.MaxValue;
                    }
                }
            
            await _db.Services.UpsertOneAsync(s => s.Id == service.Id, service);

            await _mediator.Publish(new Core.Events.ServiceRefreshed(service.Id));
        }
    }
}