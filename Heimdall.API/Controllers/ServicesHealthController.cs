using Microsoft.AspNetCore.Mvc;
using MediatR;
using System;
using System.Threading.Tasks;
using Heimdall.Analytics.Queries;

namespace Heimdall.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    public class ServicesHealthController : Controller
    {
        private readonly IMediator _mediator;

        public ServicesHealthController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [ProducesResponseType(typeof(Analytics.Queries.Models.ServiceHealth), 200)]
        public async Task<Analytics.Queries.Models.ServiceHealth> Get(string name, DateTime from, DateTime to)
        {
            var query = new ReadServiceHealth(name, from, to);
            var result = await _mediator.Send(query);

            return result;
        }
    }
}
