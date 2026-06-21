using System;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates;

/// <summary>
/// 留言墙实体 — 访客公开留言
/// </summary>
public class Message : BaseEntity<Guid>, IAggregateRoot
{
    public string Nickname { get; private set; }
    public string Content { get; private set; }
    public string? Contact { get; private set; } // 可选联系方式
    public CommentStatus Status { get; private set; } // 复用评论审核状态

    private Message() { }

    public Message(string nickname, string content, string? contact)
    {
        Nickname = nickname;
        Content = content;
        Contact = contact;
        Status = CommentStatus.Pending;
    }

    public void Audit(CommentStatus status)
    {
        Status = status;
        UpdateModifyTime();
    }
}
