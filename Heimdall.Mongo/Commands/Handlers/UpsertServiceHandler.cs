using Heimdall.Core.Commands;
using LibCore.CQRS.Commands.Handlers;
using System;
using System.Linq;
using LibCore.CQRS.Validation;
using Heimdall.Mongo.Infrastructure;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Commands.Handlers
{
    public class UpsertServiceHandler : BaseCommandHandler<UpsertService>
    {
        private IDbContext _db;

        public UpsertServiceHandler(IDbContext db, IValidator<UpsertService> validator) : base(validator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected override async Task RunCommand(UpsertService command)
        {
            var service = await _db.Services.FindOneAsync(s => s.Name == command.Name);

            service = service ?? new Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Name = command.Name
            };
            service.Endpoints = service.Endpoints ?? Enumerable.Empty<Infrastructure.Entities.ServiceEndpoint>();
            service.Endpoints = service.Endpoints.Append(new Infrastructure.Entities.ServiceEndpoint()
            {
                Active = true,
                Url = command.Endpoint
            });
            await _db.Services.UpsertOneAsync(s => s.Name == command.Name, service);
        }
    }
}
