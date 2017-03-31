using System.Collections.Generic;
using System.Threading.Tasks;
using Heimdall.Web.DTO;

namespace Heimdall.Web.Proxies
{
    public interface IServicesProxy
    {
        Task<IEnumerable<ServiceArchiveItem>> Read();
        Task<ServiceDetails> ReadDetails(string name);
        Task<ServiceDetails> Refresh(string name);
    }
}