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
            var service = await _db.Services.FindOneAsync(s => s.Name == command.ServiceName);
            if(null == service)
            {
                base.AddError(new ValidationError("service", $"Unable to load service by name: '{command.ServiceName}'"));
                return;
            }

            if (null == service.Endpoints || !service.Endpoints.Any())
                return;

            if (service.Endpoints.Any(e => e.Url == command.Endpoint))
                base.AddError(new ValidationError("endpoint", $"endpoint '{command.Endpoint}' already exists on '{command.ServiceName}'"));
        }
    }
}
