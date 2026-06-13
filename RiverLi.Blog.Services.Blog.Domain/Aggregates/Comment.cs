using System;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates;

/// <summary>
/// 评论实体 (附属实体)
/// </summary>
public class Comment : BaseEntity<Guid>, IAggregateRoot
{
    public Guid ArticleId { get; private set; }
    public string ReviewerId { get; private set; }
    public string ReviewerName { get; private set; }
    public string Content { get; private set; }
    public CommentStatus Status { get; private set; }

    private Comment()
    {
    }

    public Comment(Guid articleId, string reviewerId, string reviewerName, string content)
    {
        ArticleId = articleId;
        ReviewerId = reviewerId;
        ReviewerName = reviewerName;
        Content = content;
        Status = CommentStatus.Pending; // 默认待审核，防垃圾评论
    }

    /// <summary>
    /// 领域行为：审核评论
    /// </summary>
    public void Audit(CommentStatus targetStatus)
    {
        Status = targetStatus;
        UpdateModifyTime();
    }
}