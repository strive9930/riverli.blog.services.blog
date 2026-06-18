using System.Collections.Generic;
using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

/// <summary>
/// 分页查询文章列表 (带多条件过滤)
/// </summary>
public record GetArticlePageQuery(
    int PageIndex = 1,
    int PageSize = 10,
    string? Keyword = null,
    Guid? CategoryId = null,
    Guid? TagId = null,
    string? Status = null,
    string? SortBy = null
) : IRequest<PagedResult<ArticleDto>>;
