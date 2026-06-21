using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

/// <summary>
/// 通过 Slug 获取文章详情
/// </summary>
public record GetArticleBySlugQuery(string Slug) : IRequest<Result<ArticleDetailDto>>;
