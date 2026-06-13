using System;
using System.Collections.Generic;

namespace RiverLi.Blog.Services.Blog.Application.Common.Dto;

/// <summary>
/// 文章详情 DTO (含正文)
/// </summary>
public record ArticleDetailDto(
    Guid Id,
    string Title,
    string Content,
    string Summary,
    string? CoverUrl,
    string AuthorId,
    string AuthorName,
    string Status,
    Guid? CategoryId,
    string? CategoryName,
    List<TagDto> Tags,
    int ViewCount,
    int CommentCount,
    DateTime CreatedTime,
    DateTime? ModifiedTime
);
