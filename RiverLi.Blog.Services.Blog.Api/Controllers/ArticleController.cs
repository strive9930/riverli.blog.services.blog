
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/articles")]
    public class ArticleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ArticleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>分页查询文章列表</summary>
        [HttpGet("page")]
        public async Task<IActionResult> GetPage([FromQuery] GetArticlePageQuery query)
        {
            var result = await _mediator.Send(query);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>获取单篇文章详情</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _mediator.Send(new GetArticleDetailQuery(id));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>发布新文章</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateArticleCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>修改文章内容</summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleCommand command)
        {
            if (id != command.Id)
                return BadRequest("路由 ID 与请求体 ID 不一致");

            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>删除文章 (软删除)</summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteArticleCommand(id));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>切换文章状态 (上架/下架)</summary>
        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeArticleStatusCommand command)
        {
            if (id != command.Id)
                return BadRequest("路由 ID 与请求体 ID 不一致");

            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }
    }
}
