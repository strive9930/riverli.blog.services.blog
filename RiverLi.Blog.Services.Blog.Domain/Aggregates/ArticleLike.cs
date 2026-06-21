using System;
using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates;

/// <summary>
/// 文章点赞记录
/// </summary>
public class ArticleLike : BaseEntity<Guid>, IAggregateRoot
{
    public Guid ArticleId { get; private set; }
    public string UserId { get; private set; } // 点赞用户ID（匿名则为IP或设备指纹）

    private ArticleLike() { }

    public ArticleLike(Guid articleId, string userId)
    {
        ArticleId = articleId;
        UserId = userId;
    }
}
