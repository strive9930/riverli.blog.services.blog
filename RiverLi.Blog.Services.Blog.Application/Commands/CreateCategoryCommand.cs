using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

/// <summary>
/// 新增分类命令
/// </summary>
public record CreateCategoryCommand(
    string Name,
    string Slug,
    string? Description,
    Guid? ParentId,
    int SortOrder = 0
) : IRequest<Result<Guid>>;
