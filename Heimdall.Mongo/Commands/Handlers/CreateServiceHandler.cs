using Heimdall.Core.Commands;
using LibCore.CQRS.Commands.Handlers;
using System;
using System.Linq;
using LibCore.CQRS.Validation;
using Heimdall.Mongo.Infrastructure;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Commands.Handlers
{
    public class CreateServiceHandler : BaseCommandHandler<CreateService>
    {
        private IDbContext _db;

        public CreateServiceHandler(IDbContext db, IValidator<CreateService> validator) : base(validator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected override async Task RunCommand(CreateService command)
        {
            var service = new Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                Active = false,
                Endpoints = Enumerable.Empty<Infrastructure.Entities.ServiceEndpoint>()
            };
            
            await _db.Services.InsertOneAsync(service);
        }
    }
}
