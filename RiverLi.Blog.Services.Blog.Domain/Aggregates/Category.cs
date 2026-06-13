using System;
using System.Collections.Generic;
using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates;

/// <summary>
/// 文章分类 — 树形结构聚合根
/// </summary>
public class Category : BaseEntity<Guid>, IAggregateRoot
{
    /// <summary>分类名称</summary>
    public string Name { get; private set; }

    /// <summary>URL 友好标识</summary>
    public string Slug { get; private set; }

    /// <summary>描述</summary>
    public string? Description { get; private set; }

    /// <summary>排序权重 (越小越靠前)</summary>
    public int SortOrder { get; private set; }

    /// <summary>父分类 ID (null 表示顶级)</summary>
    public Guid? ParentId { get; private set; }

    // 导航属性
    public Category? Parent { get; private set; }
    private readonly List<Category> _children = new();
    public IReadOnlyCollection<Category> Children => _children.AsReadOnly();

    private Category() { } // EF Core

    public Category(string name, string slug, string? description, Guid? parentId, int sortOrder = 0)
    {
        Name = name;
        Slug = slug;
        Description = description;
        ParentId = parentId;
        SortOrder = sortOrder;
    }

    public void Update(string name, string slug, string? description, Guid? parentId, int sortOrder)
    {
        Name = name;
        Slug = slug;
        Description = description;
        ParentId = parentId;
        SortOrder = sortOrder;
        UpdateModifyTime();
    }
}
