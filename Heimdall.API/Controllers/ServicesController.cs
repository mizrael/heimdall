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
        /// finds the service by id and returns its details along with the list of endpoints.
        /// It will return all the endpoints regardless their status
        /// </summary>
        [HttpGet("{id}", Name = "GetById")]
        [ProducesResponseType(typeof(Core.Queries.Models.ServiceDetails), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new FindService(id);
            var result = await _mediator.Send(query);
            return this.OkOrNotFound(result);
        }

        /// <summary>
        /// creates service
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [ProducesResponseType(typeof(LibCore.Web.ErrorHandling.ApiErrorInfo), 400)]
        public async Task<IActionResult> Post([FromBody]Models.CreateService request)
        {
            if (null == request)
                throw new ArgumentNullException(nameof(request));
            var command = new Core.Commands.CreateService(Guid.NewGuid(), request.Name);
            await _mediator.Publish(command);
            return CreatedAtAction("GetById", new { name = request.Name }, null);
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
            var command = new Core.Commands.AddEndpoint(request.ServiceId, Guid.NewGuid(), request.Protocol, request.Address);
            await _mediator.Publish(command);
            return CreatedAtAction("GetById", new { name = request.ServiceId }, null);
        }

        /// <summary>
        /// updates an endpoint on a service
        /// </summary>
        [HttpPut, Route("endpoint")]
        [ProducesResponseType(typeof(LibCore.Web.ErrorHandling.ApiErrorInfo), 400)]
        public async Task<IActionResult> PutEndpoint([FromBody]Models.UpdateEndpoint request)
        {
            if (null == request)
                throw new ArgumentNullException(nameof(request));
            var command = new Core.Commands.UpdateEndpoint(request.ServiceId, request.EndpointId, request.Protocol, request.Address);
            await _mediator.Publish(command);
            return Ok();
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

            var command = new Core.Commands.RefreshServiceStatus(request.Id, 10);

            await _mediator.Publish(command);

            var query = new FindService(request.Id);
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

            var command = new Core.Commands.DeleteService(request.Id);

            await _mediator.Publish(command);

            return this.Ok();
        }

        /// <summary>
        /// remove an endpoint from a service
        /// </summary>
        [HttpDelete, Route("endpoint")]
        [ProducesResponseType(typeof(LibCore.Web.ErrorHandling.ApiErrorInfo), 400)]
        public async Task<IActionResult> DeleteEndpoint([FromBody]Models.RemoveEndpoint request)
        {
            if (null == request)
                throw new ArgumentNullException(nameof(request));
            var command = new Core.Commands.RemoveEndpoint(request.ServiceId, request.EndpointId);
            await _mediator.Publish(command);
            return this.Ok();
        }
    }
}
