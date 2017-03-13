using Heimdall.Core.Commands;
using Heimdall.Mongo.Infrastructure;
using LibCore.CQRS.Commands.Handlers;
using LibCore.CQRS.Validation;
using LibCore.Web.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Commands.Handlers
{
    class RefreshServicesStatusHandler : BaseCommandHandler<RefreshServicesStatus>
    {
        private IDbContext _db;
        private IPinger _pinger;

        public RefreshServicesStatusHandler(IDbContext db, IPinger pinger, IValidator<RefreshServicesStatus> validator) : base(validator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _pinger = pinger ?? throw new ArgumentNullException(nameof(pinger));
        }

        protected override async Task RunCommand(RefreshServicesStatus command)
        {
            var services = await _db.Services.FindAsync(s => null != s);
            if (null == services || !services.Any())
                return;

            foreach (var service in services)
            {
                service.Active = false;

                if (null != service.Endpoints && service.Endpoints.Any())
                    foreach (var endpoint in service.Endpoints)
                    {
                        var pingResult = await _pinger.PingAsync(endpoint.Url, command.Timeout);
                        service.Active |= pingResult.Success;
                        endpoint.Active = pingResult.Success;
                        endpoint.RoundtripTime = pingResult.RoundtripTime;
                    }

                await _db.Services.UpsertOneAsync(s => s.Id == service.Id, service);
            }
            
        }
    }
}