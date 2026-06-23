using System;
using System.Collections.Generic;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

/// <summary>
/// 修改文章命令
/// </summary>
public record UpdateArticleCommand(
    Guid Id,
    string Title,
    string? Slug,
    string Content,
    string Summary,
    string? CoverUrl,
    Guid CategoryId,
    List<Guid>? TagIds,
    DateTime? ScheduledPublishTime
) : IRequest<Result>;
