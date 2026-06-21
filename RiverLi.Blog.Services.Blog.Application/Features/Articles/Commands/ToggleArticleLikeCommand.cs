using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

/// <summary>
/// 文章点赞（同一用户重复点赞自动取消）
/// </summary>
public record ToggleArticleLikeCommand(Guid ArticleId, string UserId) : IRequest<Result<bool>>;
