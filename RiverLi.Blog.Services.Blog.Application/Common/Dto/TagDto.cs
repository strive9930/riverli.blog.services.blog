using System;

namespace RiverLi.Blog.Services.Blog.Application.Common.Dto;

/// <summary>
/// 标签 DTO
/// </summary>
public record TagDto(
    Guid Id,
    string Name,
    string Slug,
    int ArticleCount
);
