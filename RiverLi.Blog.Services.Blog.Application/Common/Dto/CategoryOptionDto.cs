using System;

namespace RiverLi.Blog.Services.Blog.Application.Common.Dto;

/// <summary>
/// 分类选项 DTO (扁平化，供下拉选择器使用)
/// </summary>
public record CategoryOptionDto(
    Guid Id,
    string Name,
    Guid? ParentId,
    int SortOrder,
    int Depth,
    int ArticleCount
);
