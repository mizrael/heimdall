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
            var service = await _db.Services.FindOneAsync(s => s.Name == command.Name);
            if(null == service)
            {
                base.AddError(new ValidationError("service", $"Unable to load service by name: '{command.Name}'"));
                return;
            }

            if (null == service.Endpoints)
            {
                base.AddError(new ValidationError("service", $"service '{command.Name}' has no endpoints"));
                return;
            }

            if (!service.Endpoints.Any(e => e.Url == command.Endpoint))
                base.AddError(new ValidationError("service", $"endpoint '{command.Endpoint}' doesn't exist on service '{command.Name}'"));

        }
    }
}
