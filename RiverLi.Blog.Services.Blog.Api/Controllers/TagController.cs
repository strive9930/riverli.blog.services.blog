using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/tag")]
    public class TagController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TagController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateTagCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new GetTagQuery(id));
            return result.Success ? Ok(result) : NotFound(result.Message);
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTagCommand command)
        {
            var result = await _mediator.Send(command with { Id = id });
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteTagCommand(id));
            return Ok(result);
        }

        [HttpDelete("batch")]
        [Authorize]
        public async Task<IActionResult> BatchDelete([FromBody] List<Guid> ids)
        {
            var result = await _mediator.Send(new BatchDeleteTagCommand(ids));
            return Ok(result);
        }

        [HttpPost("list")]
        public async Task<IActionResult> List()
        {
            var result = await _mediator.Send(new GetTagListQuery());
            return Ok(result);
        }

        [HttpPost("paging")]
        public async Task<IActionResult> Paging([FromBody] GetTagPagingQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("hot")]
        public async Task<IActionResult> Hot([FromQuery] int count = 10)
        {
            var result = await _mediator.Send(new GetHotTagsQuery(count));
            return Ok(result);
        }
    }
}
