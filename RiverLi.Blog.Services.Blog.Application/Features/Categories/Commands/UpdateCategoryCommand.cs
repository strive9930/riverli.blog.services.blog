using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Categories.Commands;

/// <summary>
/// 修改分类命令
/// </summary>
public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    Guid? ParentId,
    int SortOrder = 0
) : IRequest<Result>;
