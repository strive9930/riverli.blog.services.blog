using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/footprint")]
    public class FootprintController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FootprintController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateFootprintCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new GetFootprintQuery(id));
            return result.Success ? Ok(result) : NotFound(result.Message);
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFootprintCommand command)
        {
            var result = await _mediator.Send(command with { Id = id });
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteFootprintCommand(id));
            return Ok(result);
        }

        [HttpGet("/api/footprints")]
        public async Task<IActionResult> List()
        {
            var result = await _mediator.Send(new GetFootprintListQuery());
            return Ok(result);
        }
    }
}
