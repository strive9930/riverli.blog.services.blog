using System;
using RiverLi.DDD.Core.Domain.Events; // 引入您的 IDomainEvent 接口

namespace RiverLi.Blog.Services.Blog.Domain.Events;

/// <summary>
/// 文章创建成功事件
/// </summary>
/// <param name="ArticleId">文章ID</param>
/// <param name="Title">文章标题</param>
/// <param name="AuthorId">作者ID (与实体保持一致为 string 类型)</param>
public record ArticleCreatedEvent(
    Guid ArticleId, 
    string Title, 
    string AuthorId
) : IDomainEvent
{
    /// <summary>
    /// 事件发生时间（UTC）- 自动初始化，且不可篡改 (init)
    /// </summary>
    public DateTime EventTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// 事件唯一标识（用于幂等处理）- 自动初始化，且不可篡改 (init)
    /// </summary>
    public Guid EventId { get; init; } = Guid.NewGuid();
}