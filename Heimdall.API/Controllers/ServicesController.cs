using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System;
using System.Threading.Tasks;
using LibCore.Web.Extensions;
using Heimdall.Core.Queries;

namespace Heimdall.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class ServicesController : Controller
    {
        private readonly IMediator _mediator;

        public ServicesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var query = new ReadServices();
            var result = await _mediator.Send(query);
            return this.OkOrNotFound(result);
        }

        /// <summary>
        /// finds the service by name and returns its details along with the list of available endpoints.
        /// If no endpoint is active, returns null.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        //[HttpGet, Route("{name}")]
        [HttpGet("{name}", Name = "GetByName")]
        public async Task<IActionResult> Get(string name)
        {
            var query = new FindService(name);
            var result = await _mediator.Send(query);
            return this.OkOrNotFound(result);
        }

        /// <summary>
        /// inserts or updates a service
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Models.UpsertService service)
        {
            if (null == service)
                throw new ArgumentNullException(nameof(service));
            var command = new Core.Commands.UpsertService(service.Name, service.Endpoint);
            await _mediator.Publish(command);
            return CreatedAtAction("Get", new { name = service.Name }, null);
        }

        /// <summary>
        /// refreshes a registered service
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("refresh")]
        public async Task<IActionResult> PostRefresh([FromBody]string name)
        {
            var command = new Core.Commands.RefreshServiceStatus(name, 10);

            await _mediator.Publish(command);

            var query = new FindService(name);
            var result = await _mediator.Send(query);
            
            return this.Ok(result);
        }
    }
}
