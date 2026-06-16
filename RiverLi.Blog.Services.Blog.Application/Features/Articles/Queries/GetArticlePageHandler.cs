using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

public class GetArticlePageHandler : IRequestHandler<GetArticlePageQuery, Result<PagedResult<ArticleDto>>>
{
    private readonly IRepository<Article, Guid> _repository;
    private readonly IMemoryCache _cache;

    public GetArticlePageHandler(IRepository<Article, Guid> repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<Result<PagedResult<ArticleDto>>> Handle(GetArticlePageQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"articles_page_{request.PageIndex}_{request.PageSize}_{request.Keyword}_{request.CategoryId}_{request.TagId}_{request.Status}_{request.SortBy}";

        if (_cache.TryGetValue<PagedResult<ArticleDto>>(cacheKey, out var cached))
            return Result<PagedResult<ArticleDto>>.SuccessResult(cached);

        var query = _repository.AsQueryable().Where(a => !a.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            query = query.Where(a =>
                a.Title.Contains(request.Keyword) ||
                a.Summary.Contains(request.Keyword));
        }

        if (request.CategoryId.HasValue)
            query = query.Where(a => a.CategoryId == request.CategoryId.Value);

        if (request.TagId.HasValue)
            query = query.Where(a => a.Tags.Any(t => t.TagId == request.TagId.Value));

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            System.Enum.TryParse<RiverLi.Blog.Services.Blog.Domain.Enum.ArticleStatus>(request.Status, true, out var status))
            query = query.Where(a => a.Status == status);

        var totalCount = await query.CountAsync(cancellationToken);

        query = request.SortBy?.ToLower() switch
        {
            "viewcount" => query.OrderByDescending(a => a.ViewCount),
            "created" => query.OrderByDescending(a => a.CreateTime),
            _ => query.OrderByDescending(a => a.CreateTime)
        };

        var items = await query
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new ArticleDto(
                a.Id, a.Title, a.Summary, a.CoverUrl, a.AuthorName,
                a.Status.ToString(), null,
                a.Tags.Select(t => t.TagId.ToString()).ToList(),
                a.ViewCount, a.Comments.Count, a.CreateTime
            ))
            .ToListAsync(cancellationToken);

        var result = PagedResult<ArticleDto>.SuccessResult(items, totalCount, request.PageIndex, request.PageSize);
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));

        return Result<PagedResult<ArticleDto>>.SuccessResult(result);
    }
}
