using System;

namespace RiverLi.Blog.Services.Blog.Application.Common.Dto;

/// <summary>
/// 评论 DTO
/// </summary>
public record CommentDto(
    Guid Id,
    Guid ArticleId,
    string ArticleTitle,
    string ReviewerName,
    string Content,
    string Status,
    DateTime CreatedTime
);
