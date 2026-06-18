using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public record UpdateSiteNavigationCommand(
    Guid Id,
    Guid? ParentId,
    string Title,
    string LinkUrl,
    string? Icon,
    int SortOrder,
    string Target,
    bool IsVisible
) : IRequest<Result<bool>>;