using System;
using System.Collections.Generic;

namespace RiverLi.Blog.Services.Blog.Application.Common.Dto;

/// <summary>
/// 分类树节点 DTO
/// </summary>
public record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    int SortOrder,
    int ArticleCount,
    List<CategoryDto> Children
);
