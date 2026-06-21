using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.FriendLinks.Commands;

public record UpdateFriendLinkCommand(
    Guid Id,
    string SiteName,
    string SiteDescription,
    string Url,
    string? AvatarUrl,
    string Owner,
    string? RssUrl
) : IRequest<Result>;
