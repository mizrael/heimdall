using Heimdall.Core.Commands;
using Heimdall.Mongo.Infrastructure;
using LibCore.CQRS.Commands.Handlers;
using LibCore.CQRS.Validation;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Commands.Handlers
{
    public class RefreshServicesStatusHandler : BaseCommandHandler<RefreshServicesStatus>
    {
        private readonly IMediator _mediator;
        private readonly IDbContext _db;
        
        public RefreshServicesStatusHandler(IDbContext db, IMediator mediator, IValidator<RefreshServicesStatus> validator) : base(validator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        protected override async Task RunCommand(RefreshServicesStatus command)
        {
            var services = await _db.Services.FindAsync(s => null != s);
            if (null == services || !services.Any())
                return;

            foreach (var service in services)
            {
                await _mediator.Publish(new RefreshServiceStatus(service.Id, command.Timeout));
            }
            
        }
    }
}