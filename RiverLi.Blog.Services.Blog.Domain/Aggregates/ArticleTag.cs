using System;
using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates;

/// <summary>
/// 文章与标签的映射实体
/// </summary>
public class ArticleTag : BaseEntity<Guid>, IAggregateRoot
{
    public Guid ArticleId { get; private set; }
    public Guid TagId { get; private set; }

    private ArticleTag() { }

    internal ArticleTag(Guid articleId, Guid tagId)
    {
        ArticleId = articleId;
        TagId = tagId;
    }
}