using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/cate")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoryController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new GetCategoryQuery(id));
            return result.Success ? Ok(result) : NotFound(result.Message);
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
        {
            var result = await _mediator.Send(command with { Id = id });
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id));
            return Ok(result);
        }

        [HttpDelete("batch")]
        [Authorize]
        public async Task<IActionResult> BatchDelete([FromBody] List<Guid> ids)
        {
            var result = await _mediator.Send(new BatchDeleteCategoryCommand(ids));
            return Ok(result);
        }

        [HttpPost("list")]
        public async Task<IActionResult> List()
        {
            var result = await _mediator.Send(new GetCategoryListQuery());
            return Ok(result);
        }

        [HttpPost("paging")]
        public async Task<IActionResult> Paging([FromBody] GetCategoryPagingQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("tree")]
        public async Task<IActionResult> Tree()
        {
            var result = await _mediator.Send(new GetCategoryTreeQuery());
            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> ArticleCount()
        {
            var result = await _mediator.Send(new GetCategoryArticleCountQuery());
            return Ok(result);
        }
    }
}
