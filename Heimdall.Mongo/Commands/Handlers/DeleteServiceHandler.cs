using Heimdall.Core.Commands;
using LibCore.CQRS.Commands.Handlers;
using System;
using LibCore.CQRS.Validation;
using Heimdall.Mongo.Infrastructure;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Commands.Handlers
{
    public class DeleteServiceHandler : BaseCommandHandler<DeleteService>
    {
        private IDbContext _db;

        public DeleteServiceHandler(IDbContext db, IValidator<DeleteService> validator) : base(validator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected override async Task RunCommand(DeleteService command)
        {
            await _db.Services.DeleteOneAsync(s => s.Name == command.Name);
        }
    }
}
