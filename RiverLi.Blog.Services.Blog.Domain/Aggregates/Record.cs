using System;
using System.Collections.Generic;
using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates;

/// <summary>
/// 说说/动态 — 类微博短内容
/// </summary>
public class Record : BaseEntity<Guid>, IAggregateRoot
{
    public string Content { get; private set; }
    public string? ImageUrls { get; private set; } // 逗号分隔的多图URL
    public string? Location { get; private set; }
    public bool IsPublic { get; private set; }

    private Record() { }

    public Record(string content, string? imageUrls, string? location, bool isPublic = true)
    {
        Content = content;
        ImageUrls = imageUrls;
        Location = location;
        IsPublic = isPublic;
    }

    public void Update(string content, string? imageUrls, string? location, bool isPublic)
    {
        Content = content;
        ImageUrls = imageUrls;
        Location = location;
        IsPublic = isPublic;
        UpdateModifyTime();
    }
}
