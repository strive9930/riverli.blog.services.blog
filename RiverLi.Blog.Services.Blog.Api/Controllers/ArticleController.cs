using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/article")]
    public class ArticleController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ArticleController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateArticleCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, [FromQuery] string? password = null)
        {
            var result = await _mediator.Send(new GetArticleQuery(id, password));
            return result.Success ? Ok(result) : NotFound(result.Message);
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleCommand command)
        {
            var result = await _mediator.Send(command with { Id = id });
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteArticleCommand(id));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpDelete("batch")]
        [Authorize]
        public async Task<IActionResult> BatchDelete([FromBody] List<Guid> ids)
        {
            var result = await _mediator.Send(new BatchDeleteArticleCommand(ids));
            return Ok(result);
        }

        [HttpPost("list")]
        public async Task<IActionResult> List([FromBody] GetArticleListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("paging")]
        public async Task<IActionResult> Paging([FromBody] GetArticlePagingQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPatch("view/{id}")]
        public async Task<IActionResult> IncrementView(Guid id)
        {
            var result = await _mediator.Send(new IncrementViewCountCommand(id));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("random")]
        public async Task<IActionResult> Random([FromQuery] int count = 5)
        {
            var result = await _mediator.Send(new GetRandomArticlesQuery(count));
            return Ok(result);
        }

        [HttpGet("archive")]
        public async Task<IActionResult> Archive()
        {
            var result = await _mediator.Send(new GetArticleArchiveQuery());
            return Ok(result);
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchArticlesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
