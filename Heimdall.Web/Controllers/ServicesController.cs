using Heimdall.Web.Proxies;
using LibCore.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Heimdall.Web.Controllers
{
    [Route("api/[controller]")]
    public class ServicesController : Controller
    {
        private readonly IServicesProxy _servicesProxy;

        public ServicesController(IServicesProxy apiClient)
        {
            _servicesProxy = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> Get()
        {
            var result = await _servicesProxy.Read();
            return this.OkOrNotFound(result);
        }
    }
}
