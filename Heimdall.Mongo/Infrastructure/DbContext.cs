using LibCore.Mongo;
using MongoDB.Driver;
using System;

namespace Heimdall.Mongo.Infrastructure
{
    public class DbContext : IDbContext
    {
        public DbContext(IRepositoryFactory repoFactory, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));
            if (null == repoFactory)
                throw new ArgumentNullException(nameof(repoFactory));

            this.Services = repoFactory.Create<Entities.Service>(new RepositoryOptions(connectionString, "services"));
            var ixb = new IndexKeysDefinitionBuilder<Entities.Service>();
            this.Services.CreateIndex(ixb.Ascending(u => u.Name));
        }

        public IRepository<Entities.Service> Services { get; private set; }
    }

}
