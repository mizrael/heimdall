using Heimdall.Core.Commands;
using Heimdall.Mongo.Infrastructure;
using LibCore.CQRS.Validation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Commands.Validation
{
    public class UpdateEndpointValidator : Validator<UpdateEndpoint>
    {
        private IDbContext _db;

        public UpdateEndpointValidator(IDbContext db) 
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected override async Task RunAsync(UpdateEndpoint command)
        {
            var service = await _db.Services.FindOneAsync(s => s.Name == command.ServiceName);
            if(null == service)
            {
                base.AddError(new ValidationError("service", $"Unable to load service by name: '{command.ServiceName}'"));
                return;
            }

            if (null == service.Endpoints || !service.Endpoints.Any())
            {
                base.AddError(new ValidationError("service", $"Service '{command.ServiceName}' has no endpoints"));
                return;
            }

            if (service.Endpoints.Any(e => e.Id != command.EndpointId))
                base.AddError(new ValidationError("endpoint", $"endpoint  with Id'{command.EndpointId}' not found on '{command.ServiceName}'"));
            
        }
    }
}
