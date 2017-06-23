using LibCore.Mongo;
using MongoDB.Driver;
using System;

namespace Heimdall.Analytics.Mongo.Infrastructure
{
    public class AnalyticsDbContext : IAnalyticsDbContext
    {
        public AnalyticsDbContext(IRepositoryFactory repoFactory, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));
            if (null == repoFactory)
                throw new ArgumentNullException(nameof(repoFactory));

            this.ServicesHealth = repoFactory.Create<Entities.ServiceHealth>(new RepositoryOptions(connectionString, "servicesHealth"));
            var servicesIxb = new IndexKeysDefinitionBuilder<Entities.ServiceHealth>();
            var indexKeys = servicesIxb.Ascending(u => u.ServiceId).Ascending(u => u.TimestampMinute);
            this.ServicesHealth.CreateIndex(indexKeys, new CreateIndexOptions() { Unique = true });
        }

        public IRepository<Entities.ServiceHealth> ServicesHealth { get; private set; }
    }

}
