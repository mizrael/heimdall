using LibCore.Mongo;

namespace Heimdall.Analytics.Mongo.Infrastructure
{

    public interface IAnalyticsDbContext
    {
        IRepository<Entities.ServiceHealth> ServicesHealth
        {
            get;
        }
    }

}