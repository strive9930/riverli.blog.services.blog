using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public record CreateSiteNavigationCommand(
    Guid? ParentId,
    string Title,
    string LinkUrl,
    string? Icon,
    int SortOrder,
    string Target = "_self",
    bool IsVisible = true
) : IRequest<Result<Guid>>;