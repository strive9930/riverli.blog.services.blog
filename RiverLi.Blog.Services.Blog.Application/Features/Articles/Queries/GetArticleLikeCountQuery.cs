using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

/// <summary>
/// 查询文章点赞数
/// </summary>
public record GetArticleLikeCountQuery(Guid ArticleId) : IRequest<Result<int>>;

/// <summary>
/// 检查用户是否已点赞
/// </summary>
public record CheckUserLikedQuery(Guid ArticleId, string UserId) : IRequest<Result<bool>>;
