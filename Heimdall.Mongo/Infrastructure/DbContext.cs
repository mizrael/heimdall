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
            var servicesIxb = new IndexKeysDefinitionBuilder<Entities.Service>();
            this.Services.CreateIndex(servicesIxb.Ascending(u => u.Name), new CreateIndexOptions() { Unique = true });

            this.TraceEvents = repoFactory.Create<Entities.TraceEvent>(new RepositoryOptions(connectionString, "events"));
            var eventsIxb = new IndexKeysDefinitionBuilder<Entities.TraceEvent>();
            this.TraceEvents.CreateIndex(eventsIxb.Ascending(u => u.Name), new CreateIndexOptions() { Unique = true });
        }

        public IRepository<Entities.TraceEvent> TraceEvents { get; private set; }
        public IRepository<Entities.Service> Services { get; private set; }
    }

}
