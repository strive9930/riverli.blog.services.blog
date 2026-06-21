
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Infrastructure.Shared.Controllers;
using RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;
using RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/blog/[controller]")]
    public class ArticleController : BaseApiController
    {
        private readonly IMediator _mediator;

        public ArticleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>公开：分页查询文章列表（仅返回已发布）</summary>
        [AllowAnonymous]
        [HttpGet("page")]
        public async Task<IActionResult> GetPage([FromQuery] GetArticlePageQuery query)
        {
            var result = await _mediator.Send(query);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>公开：获取单篇文章详情</summary>
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _mediator.Send(new GetArticleDetailQuery(id));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>公开：通过 Slug 获取文章详情</summary>
        [AllowAnonymous]
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var result = await _mediator.Send(new GetArticleBySlugQuery(slug));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>公开：递增文章阅读量</summary>
        [AllowAnonymous]
        [HttpPost("{id:guid}/view")]
        public async Task<IActionResult> IncrementView(Guid id)
        {
            var result = await _mediator.Send(new IncrementArticleViewCountCommand(id));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>公开：随机推荐文章</summary>
        [AllowAnonymous]
        [HttpGet("random")]
        public async Task<IActionResult> GetRandom([FromQuery] int count = 5)
        {
            var result = await _mediator.Send(new GetRandomArticlesQuery(count));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>公开：全局最新评论</summary>
        [AllowAnonymous]
        [HttpGet("latest-comments")]
        public async Task<IActionResult> GetLatestComments([FromQuery] int count = 5)
        {
            var result = await _mediator.Send(new GetLatestCommentsQuery(count));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>公开：点赞/取消点赞</summary>
        [AllowAnonymous]
        [HttpPost("{id:guid}/like")]
        public async Task<IActionResult> ToggleLike(Guid id, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("userId 不能为空");
            var result = await _mediator.Send(new ToggleArticleLikeCommand(id, userId));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>公开：查询点赞数</summary>
        [AllowAnonymous]
        [HttpGet("{id:guid}/like-count")]
        public async Task<IActionResult> GetLikeCount(Guid id)
        {
            var result = await _mediator.Send(new GetArticleLikeCountQuery(id));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>公开：检查用户是否已点赞</summary>
        [AllowAnonymous]
        [HttpGet("{id:guid}/liked")]
        public async Task<IActionResult> CheckLiked(Guid id, [FromQuery] string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("userId 不能为空");
            var result = await _mediator.Send(new CheckUserLikedQuery(id, userId));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>发布新文章（需登录）</summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateArticleCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>修改文章内容（需登录）</summary>
        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleCommand command)
        {
            if (id != command.Id)
                return BadRequest("路由 ID 与请求体 ID 不一致");

            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>删除文章 - 软删除（需登录）</summary>
        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteArticleCommand(id));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>切换文章状态 - 上架/下架（需登录）</summary>
        [Authorize]
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
