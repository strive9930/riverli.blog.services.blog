using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/rss")]
    public class RssController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RssController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateRssCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new GetRssQuery(id));
            return result.Success ? Ok(result) : NotFound(result.Message);
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRssCommand command)
        {
            var result = await _mediator.Send(command with { Id = id });
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteRssCommand(id));
            return Ok(result);
        }

        [HttpGet("/api/rsses")]
        public async Task<IActionResult> List()
        {
            var result = await _mediator.Send(new GetRssListQuery());
            return Ok(result);
        }

        [HttpPost("fetch/{id}")]
        [Authorize]
        public async Task<IActionResult> Fetch(Guid id)
        {
            var result = await _mediator.Send(new FetchRssCommand(id));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }
    }
}
