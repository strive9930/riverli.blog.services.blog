using System.Collections.Generic;
using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

/// <summary>
/// 随机推荐 N 篇已发布文章
/// </summary>
public record GetRandomArticlesQuery(int Count = 5) : IRequest<Result<List<ArticleDto>>>;
