using System;

namespace RiverLi.Blog.Services.Blog.Application.Common.Dto;

public record FriendLinkDto(
    Guid Id,
    string SiteName,
    string SiteDescription,
    string Url,
    string? AvatarUrl,
    string Owner,
    string? RssUrl,
    string Status,
    int SortOrder,
    bool IsTop,
    DateTime CreatedTime
);
