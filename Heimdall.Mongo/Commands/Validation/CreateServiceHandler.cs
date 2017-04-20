﻿using Heimdall.Core.Commands;
using Heimdall.Mongo.Infrastructure;
using LibCore.CQRS.Validation;
using System;
using System.Threading.Tasks;

namespace Heimdall.Mongo.Commands.Validation
{
    public class CreateServiceHandler : Validator<CreateService>
    {
        private IDbContext _db;

        public CreateServiceHandler(IDbContext db) 
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        protected override async Task RunAsync(CreateService command)
        {
            var service = await _db.Services.FindOneAsync(s => s.Name == command.Name);
            if(null != service)
            {
                base.AddError(new ValidationError("service", $"service '{command.Name}' already exists"));
            }
        }
    }
}
