using System;

namespace RiverLi.Blog.Services.Blog.Application.Common.Dto;

public record SiteNavigationDto
{
    public Guid Id { get; init; }
    public Guid? ParentId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string LinkUrl { get; init; } = string.Empty;
    public string? Icon { get; init; }
    public int SortOrder { get; init; }
    public string Target { get; init; } = string.Empty;
    public bool IsVisible { get; init; }
    public List<SiteNavigationDto> Children { get; set; } = new();
}