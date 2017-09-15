using Heimdall.Core.Commands;
using Heimdall.Mongo.Infrastructure;
using LibCore.CQRS.Validation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Commands.Validation
{
    public class AddEndpointValidator : Validator<AddEndpoint>
    {
        private IDbContext _db;

        public AddEndpointValidator(IDbContext db) 
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected override async Task RunAsync(AddEndpoint command)
        {
            var service = await _db.Services.FindOneAsync(s => s.Id == command.ServiceId);
            if (null == service)
            {
                base.AddError(new ValidationError("service", $"Unable to load service by id: '{command.ServiceId}'"));
                return;
            }

            if (null == service.Endpoints || !service.Endpoints.Any())
                return;

            if (service.Endpoints.Any(e => e.Id == command.EndpointId))
                base.AddError(new ValidationError("endpoint", $"endpoint  with Id '{command.EndpointId}' already exists on service '{command.ServiceId}'"));

            if (service.Endpoints.Any(e => e.Address == command.Address && e.Protocol == command.Protocol))
                base.AddError(new ValidationError("endpoint", $"endpoint '{command.Address}' already exists on service '{command.ServiceId}'"));
        }
    }
}
