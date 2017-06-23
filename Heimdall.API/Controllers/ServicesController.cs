using Microsoft.AspNetCore.Mvc;
using MediatR;
using System;
using System.Threading.Tasks;
using LibCore.Web.Extensions;
using Heimdall.Core.Queries;
using System.Collections.Generic;

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

        /// <summary>
        /// returns the list of all registered services
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Core.Queries.Models.ServiceArchiveItem>), 200)]
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
        [HttpGet("{name}", Name = "GetByName")]
        [ProducesResponseType(typeof(Core.Queries.Models.ServiceDetails), 200)]
        public async Task<IActionResult> GetByName(string name)
        {
            var query = new FindService(name, false);
            var result = await _mediator.Send(query);
            return this.OkOrNotFound(result);
        }

        /// <summary>
        /// finds the service by name and returns its details along with the list of available endpoints.
        /// It will return all the endpoints regardless their status
        /// </summary>
        [HttpGet("{name}/force", Name = "GetByNameFull")]
        [ProducesResponseType(typeof(Core.Queries.Models.ServiceDetails), 200)]
        public async Task<IActionResult> GetByNameFull(string name)
        {
            var query = new FindService(name, true);
            var result = await _mediator.Send(query);
            return this.OkOrNotFound(result);
        }

        /// <summary>
        /// creates service
        /// </summary>
        /// <param name="service"></param>
        [HttpPost]
        [ProducesResponseType(typeof(LibCore.Web.ErrorHandling.ApiErrorInfo), 400)]
        public async Task<IActionResult> Post([FromBody]Models.CreateService service)
        {
            if (null == service)
                throw new ArgumentNullException(nameof(service));
            var command = new Core.Commands.CreateService(service.Name, service.Endpoint);
            await _mediator.Publish(command);
            return CreatedAtAction("GetByName", new { name = service.Name }, null);
        }

        /// <summary>
        /// adds an endpoint to a service
        /// </summary>
        [HttpPost, Route("endpoint")]
        [ProducesResponseType(typeof(LibCore.Web.ErrorHandling.ApiErrorInfo), 400)]
        public async Task<IActionResult> PostEndpoint([FromBody]Models.AddEndpoint request)
        {
            if (null == request)
                throw new ArgumentNullException(nameof(request));
            var command = new Core.Commands.AddEndpoint(request.ServiceName, request.Endpoint);
            await _mediator.Publish(command);
            return CreatedAtAction("GetByName", new { name = request.ServiceName }, null);
        }

        /// <summary>
        /// refreshes a registered service
        /// </summary>
        [HttpPost, Route("refresh")]
        [ProducesResponseType(typeof(LibCore.Web.ErrorHandling.ApiErrorInfo), 400)]
        public async Task<IActionResult> PostRefresh([FromBody]Models.RefreshService request)
        {
            if (null == request)
                throw new ArgumentNullException(nameof(request));

            var command = new Core.Commands.RefreshServiceStatus(request.Name, 10);

            await _mediator.Publish(command);

            var query = new FindService(request.Name, false);
            var result = await _mediator.Send(query);
            
            return this.Ok(result);
        }

        /// <summary>
        /// deletes a registered service
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(typeof(LibCore.Web.ErrorHandling.ApiErrorInfo), 400)]
        public async Task<IActionResult> Delete([FromBody]Models.DeleteService request)
        {
            if (null == request)
                throw new ArgumentNullException(nameof(request));

            var command = new Core.Commands.DeleteService(request.Name);

            await _mediator.Publish(command);

            return this.Ok();
        }

        /// <summary>
        /// remove an endpoint from a service
        /// </summary>
        [HttpDelete, Route("endpoint")]
        [ProducesResponseType(typeof(LibCore.Web.ErrorHandling.ApiErrorInfo), 400)]
        public async Task<IActionResult> DeleteEndpoint([FromBody]Models.RemoveEndpoint model)
        {
            if (null == model)
                throw new ArgumentNullException(nameof(model));
            var command = new Core.Commands.RemoveEndpoint(model.ServiceName, model.Endpoint);
            await _mediator.Publish(command);
            return this.Ok();
        }
    }
}
