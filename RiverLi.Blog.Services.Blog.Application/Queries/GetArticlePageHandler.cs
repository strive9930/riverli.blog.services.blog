using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Infrastructure.Data;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Queries;

public class GetArticlePageHandler : IRequestHandler<GetArticlePageQuery, Result<PagedResult<ArticleDto>>>
{
    private readonly BlogDbContext _dbContext;

    public GetArticlePageHandler(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<ArticleDto>>> Handle(GetArticlePageQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Articles
            .AsNoTracking()
            .Where(a => !a.IsDeleted);

        // 关键词过滤
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            query = query.Where(a =>
                a.Title.Contains(request.Keyword) ||
                a.Summary.Contains(request.Keyword));
        }

        // 分类过滤
        if (request.CategoryId.HasValue)
        {
            query = query.Where(a => a.CategoryId == request.CategoryId.Value);
        }

        // 标签过滤
        if (request.TagId.HasValue)
        {
            query = query.Where(a => a.Tags.Any(t => t.TagId == request.TagId.Value));
        }

        // 状态过滤
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            System.Enum.TryParse<RiverLi.Blog.Services.Blog.Domain.Enum.ArticleStatus>(request.Status, true, out var status))
        {
            query = query.Where(a => a.Status == status);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // 排序
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
                a.Id,
                a.Title,
                a.Summary,
                a.CoverUrl,
                a.AuthorName,
                a.Status.ToString(),
                null, // CategoryName — 需要 JOIN，查询优化时可补
                a.Tags.Select(t => t.TagId.ToString()).ToList(),
                a.ViewCount,
                a.Comments.Count,
                a.CreateTime
            ))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<ArticleDto>>.SuccessResult(
            PagedResult<ArticleDto>.SuccessResult(items, totalCount, request.PageIndex, request.PageSize));
    }
}
