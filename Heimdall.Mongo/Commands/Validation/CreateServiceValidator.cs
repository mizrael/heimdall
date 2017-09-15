using Heimdall.Core.Commands;
using Heimdall.Mongo.Infrastructure;
using LibCore.CQRS.Validation;
using System;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Commands.Validation
{
    public class CreateServiceValidator : Validator<CreateService>
    {
        private IDbContext _db;

        public CreateServiceValidator(IDbContext db) 
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected override async Task RunAsync(CreateService command)
        {
            var service = await _db.Services.FindOneAsync(s => s.Id == command.ServiceId);
            if (null != service)
                base.AddError(new ValidationError("service", $"service with id '{command.ServiceId}' already exists"));

            service = await _db.Services.FindOneAsync(s => s.Name == command.ServiceName);
            if(null != service)
                base.AddError(new ValidationError("service", $"service with name '{command.ServiceName}' already exists"));
        }
    }
}
