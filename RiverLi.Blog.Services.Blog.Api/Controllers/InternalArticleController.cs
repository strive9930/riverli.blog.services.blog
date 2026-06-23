using MediatR;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;
using RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.Blog.Infrastructure.Shared.Controllers;

namespace RiverLi.Blog.Services.Blog.Api.Controllers;

/// <summary>
/// 内部接口 — 供 Quartz 调度服务回调，需通过网关 X-Internal-Secret 鉴权
/// </summary>
[ApiController]
[Route("api/blog/internal/article")]
public class InternalArticleController : BaseApiController
{
    private readonly IMediator _mediator;

    public InternalArticleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 发布所有到期的定时文章（由 Quartz Job 每分钟调用）
    /// </summary>
    [HttpPost("publish-scheduled")]
    public async Task<IActionResult> PublishScheduledArticles()
    {
        var query = new GetScheduledArticlesQuery();
        var result = await _mediator.Send(query);

        if (!result.Success)
            return BadRequest(result.Message);

        var publishedCount = 0;
        foreach (var articleId in result.Data)
        {
            var changeResult = await _mediator.Send(
                new ChangeArticleStatusCommand(articleId, ArticleStatus.Published));
            if (changeResult.Success) publishedCount++;
        }

        return Ok(new { Succeeded = true, PublishedCount = publishedCount });
    }
}
