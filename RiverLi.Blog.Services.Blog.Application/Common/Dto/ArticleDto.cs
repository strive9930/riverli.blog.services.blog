using System;
using System.Collections.Generic;

namespace RiverLi.Blog.Services.Blog.Application.Common.Dto;

/// <summary>
/// 文章列表项 DTO
/// </summary>
public record ArticleDto(
    Guid Id,
    string Title,
    string Slug,
    string Summary,
    string? CoverUrl,
    string AuthorName,
    string Status,
    string? CategoryName,
    List<string> Tags,
    int ViewCount,
    int CommentCount,
    DateTime CreatedTime
);
