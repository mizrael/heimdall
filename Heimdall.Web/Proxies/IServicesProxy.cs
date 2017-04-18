using System.Collections.Generic;
using System.Threading.Tasks;
using Heimdall.Web.DTO;

namespace Heimdall.Web.Proxies
{
    public interface IServicesProxy
    {
        Task<IEnumerable<ServiceArchiveItem>> ReadAsync();
        Task<ServiceDetails> ReadDetailsAsync(string name);
        Task<ServiceDetails> RefreshAsync(string name);
        Task CreateAsync(CreateService dto);
        Task DeleteAsync(string name);
    }
}