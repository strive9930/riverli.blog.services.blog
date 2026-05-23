using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/wall")]
    public class WallController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WallController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWallCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new GetWallQuery(id));
            return result.Success ? Ok(result) : NotFound(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteWallCommand(id));
            return Ok(result);
        }

        [HttpDelete("batch")]
        [Authorize]
        public async Task<IActionResult> BatchDelete([FromBody] List<Guid> ids)
        {
            var result = await _mediator.Send(new BatchDeleteWallCommand(ids));
            return Ok(result);
        }

        [HttpPost("list")]
        public async Task<IActionResult> List()
        {
            var result = await _mediator.Send(new GetWallListQuery());
            return Ok(result);
        }

        [HttpPost("paging")]
        [Authorize]
        public async Task<IActionResult> Paging([FromBody] GetWallPagingQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("cates")]
        public async Task<IActionResult> Cates()
        {
            var result = await _mediator.Send(new GetWallCatesQuery());
            return Ok(result);
        }
    }
}
