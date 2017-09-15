using Heimdall.Core.Commands;
using Heimdall.Mongo.Infrastructure;
using LibCore.CQRS.Validation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Commands.Validation
{
    public class RemoveEndpointValidator : Validator<RemoveEndpoint>
    {
        private IDbContext _db;

        public RemoveEndpointValidator(IDbContext db) 
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected override async Task RunAsync(RemoveEndpoint command)
        {
            var service = await _db.Services.FindOneAsync(s => s.Id == command.ServiceId);
            if (null == service)
            {
                base.AddError(new ValidationError("service", $"Unable to load service by id: '{command.ServiceId}'"));
                return;
            }

            if (null == service.Endpoints)
            {
                base.AddError(new ValidationError("service", $"service '{command.ServiceId}' has no endpoints"));
                return;
            }

            if (!service.Endpoints.Any(e => e.Id == command.EndpointId))
                base.AddError(new ValidationError("endpoint", $"endpoint '{command.EndpointId}' doesn't exist on service '{command.ServiceId}'"));

        }
    }
}
