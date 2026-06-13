using System;
using System.Collections.Generic;
using System.Linq;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Domain.Common;
using RiverLi.Blog.Services.Blog.Domain.Events;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates;

/// <summary>
/// 文章聚合根
/// </summary>
public class Article : BaseEntity<Guid>, IAggregateRoot
{
    public string Title { get; private set; }
    public string Content { get; private set; } // 推荐存 Markdown 原文
    public string Summary { get; private set; }
    public string? CoverUrl { get; private set; }
    public int ViewCount { get; private set; }
    
    // 状态与分类
    public ArticleStatus Status { get; private set; }
    public Guid CategoryId { get; private set; }

    // 作者冗余信息
    public string AuthorId { get; private set; }
    public string AuthorName { get; private set; }

    // 导航属性：仅对外暴露只读集合，保护内部数据不被非法添加
    private readonly List<Comment> _comments = new();
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    private readonly List<ArticleTag> _tags = new();
    public IReadOnlyCollection<ArticleTag> Tags => _tags.AsReadOnly();

    private Article() { } 

    public Article(string title, string content, string summary, string? coverUrl, Guid categoryId, string authorId, string authorName)
    {
        Title = title;
        Content = content;
        Summary = summary;
        CoverUrl = coverUrl;
        CategoryId = categoryId;
        AuthorId = authorId;
        AuthorName = authorName;
        ViewCount = 0;
        Status = ArticleStatus.Draft; // 新建默认草稿，后续通过发布接口转换状态

        // 触发领域事件
        AddDomainEvent(new ArticleCreatedEvent(Id, Title, AuthorId));
    }

    /// <summary>
    /// 领域行为：更新文章基础内容
    /// </summary>
    public void Update(string title, string content, string summary, string? coverUrl, Guid categoryId)
    {
        Title = title;
        Content = content;
        Summary = summary;
        CoverUrl = coverUrl;
        CategoryId = categoryId;

        UpdateModifyTime();
    }

    /// <summary>
    /// 领域行为：切换文章状态 (上下架)
    /// </summary>
    public void ChangeStatus(ArticleStatus status)
    {
        Status = status;
        UpdateModifyTime();
    }

    /// <summary>
    /// 领域行为：增加浏览量
    /// </summary>
    public void IncrementViewCount()
    {
        ViewCount++;
    }

    /// <summary>
    /// 领域行为：通过聚合根添加评论
    /// </summary>
    public void AddComment(string reviewerId, string reviewerName, string content)
    {
        var comment = new Comment(Id, reviewerId, reviewerName, content);
        _comments.Add(comment);
    }

    /// <summary>
    /// 领域行为：重置并绑定新标签
    /// </summary>
    public void SetTags(IEnumerable<Guid> tagIds)
    {
        _tags.Clear(); // 暴力清理旧标签映射
        foreach (var tagId in tagIds.Distinct())
        {
            _tags.Add(new ArticleTag(Id, tagId));
        }
        UpdateModifyTime();
    }
}