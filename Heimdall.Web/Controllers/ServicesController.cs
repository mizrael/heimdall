using Heimdall.Web.DTO;
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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _servicesProxy.ReadAsync();
            return this.OkOrNotFound(result);
        }

        [HttpGet("{name}", Name = "GetByName")]
        public async Task<IActionResult> GetByName(string name)
        {
            var result = await _servicesProxy.ReadDetailsAsync(name);
            return this.OkOrNotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CreateService service)
        {
            if (null == service)
                throw new ArgumentNullException(nameof(service));
            await _servicesProxy.CreateAsync(service);
            return CreatedAtAction("GetByName", new { name = service.Name }, null);
        }

        [HttpPost, Route("endpoint")]
        public async Task<IActionResult> PostEndpoint([FromBody]AddEndpoint model)
        {
            if (null == model)
                throw new ArgumentNullException(nameof(model));

            await _servicesProxy.AddEndpoint(model);

            return CreatedAtAction("GetByName", new { name = model.ServiceName }, null);
        }

        [HttpPost, Route("refresh")]
        public async Task<IActionResult> PostRefresh([FromBody]string name)
        {
            var service = await _servicesProxy.RefreshAsync(name);
            return this.Ok(service);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]string name)
        {
            await _servicesProxy.DeleteAsync(name);

            return this.Ok();
        }

        [HttpDelete, Route("endpoint")]
        public async Task<IActionResult> DeleteEndpoint([FromBody]RemoveEndpoint model)
        {
            if (null == model)
                throw new ArgumentNullException(nameof(model));

            await _servicesProxy.DeleteEndpoint(model);

            return this.Ok();
        }
    }
}
