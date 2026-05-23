using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/comment")]
    public class CommentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CommentController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new GetCommentQuery(id));
            return result.Success ? Ok(result) : NotFound(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCommentCommand(id));
            return Ok(result);
        }

        [HttpDelete("batch")]
        [Authorize]
        public async Task<IActionResult> BatchDelete([FromBody] List<Guid> ids)
        {
            var result = await _mediator.Send(new BatchDeleteCommentCommand(ids));
            return Ok(result);
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] GetCommentListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("paging")]
        [Authorize]
        public async Task<IActionResult> Paging([FromBody] GetCommentPagingQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPatch("audit/{id}")]
        [Authorize]
        public async Task<IActionResult> Audit(Guid id, [FromBody] AuditCommentCommand command)
        {
            var result = await _mediator.Send(command with { });
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost("reply")]
        public async Task<IActionResult> Reply([FromBody] ReplyCommentCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }
    }
}
