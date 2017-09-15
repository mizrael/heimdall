using Heimdall.Core.Commands;
using LibCore.CQRS.Commands.Handlers;
using System;
using System.Linq;
using LibCore.CQRS.Validation;
using Heimdall.Mongo.Infrastructure;
using System.Threading.Tasks;


namespace Heimdall.Mongo.Commands.Handlers
{
    public class UpdateEndpointHandler : BaseCommandHandler<UpdateEndpoint>
    {
        private IDbContext _db;

        public UpdateEndpointHandler(IDbContext db, IValidator<UpdateEndpoint> validator) : base(validator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected override async Task RunCommand(UpdateEndpoint command)
        {
            var service = await _db.Services.FindOneAsync(s => s.Id == command.ServiceId);

            var endpoint = service.Endpoints.First(se => se.Id == command.EndpointId);

            endpoint.Active = false;
            endpoint.Address = command.Address;
            endpoint.Protocol = command.Protocol;

            await _db.Services.UpsertOneAsync(s => s.Id == command.ServiceId, service);
        }
    }
}
