using System;
using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates;

/// <summary>
/// 标签 — 扁平结构聚合根
/// </summary>
public class Tag : BaseEntity<Guid>, IAggregateRoot
{
    /// <summary>标签名称</summary>
    public string Name { get; private set; }

    /// <summary>URL 友好标识</summary>
    public string Slug { get; private set; }

    private Tag() { } // EF Core

    public Tag(string name, string slug)
    {
        Name = name;
        Slug = slug;
    }

    public void Update(string name, string slug)
    {
        Name = name;
        Slug = slug;
        UpdateModifyTime();
    }
}
