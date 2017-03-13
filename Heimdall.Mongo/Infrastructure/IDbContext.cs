using LibCore.Mongo;

namespace Heimdall.Mongo.Infrastructure
{

    public interface IDbContext
    {
        IRepository<Entities.Service> Services
        {
            get;
        }
    }

}