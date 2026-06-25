using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

/// <summary>
/// 文章分页查询处理器
/// 支持关键字搜索、分类/标签筛选、状态过滤、排序，结果通过 IMemoryCache 本地缓存 1 分钟
/// </summary>
public class GetArticlePageHandler : IRequestHandler<GetArticlePageQuery, PagedResult<ArticleDto>>
{
    private readonly IRepository<Article, Guid> _repository;
    private readonly IMemoryCache _cache;

    public GetArticlePageHandler(IRepository<Article, Guid> repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<PagedResult<ArticleDto>> Handle(GetArticlePageQuery request, CancellationToken cancellationToken)
    {
        // 1. 构造缓存 Key：组合所有查询参数，确保不同查询条件命中不同缓存
        var cacheKey = $"articles_page_{request.PageIndex}_{request.PageSize}_{request.Keyword}_{request.CategoryId}_{request.TagId}_{request.Status}_{request.SortBy}";

        // 2. TODO: 缓存暂时关闭
        // if (_cache.TryGetValue<PagedResult<ArticleDto>>(cacheKey, out var cached))
        //     return cached;

        // 3. 构建基础查询：排除已软删除的文章
        var query = _repository.AsQueryable().Where(a => !a.IsDeleted);

        // 4. 关键词模糊搜索 (标题 + 摘要)
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            query = query.Where(a =>
                a.Title.Contains(request.Keyword) ||
                a.Summary.Contains(request.Keyword));
        }

        // 5. 按分类筛选
        if (request.CategoryId.HasValue)
            query = query.Where(a => a.CategoryId == request.CategoryId.Value);

        // 6. 按标签筛选：通过中间表 ArticleTag 的 TagId 匹配
        if (request.TagId.HasValue)
            query = query.Where(a => a.Tags.Any(t => t.TagId == request.TagId.Value));

        // 7. 按发布状态筛选 (Draft / Published)；未传参数则查全部
        if (!string.IsNullOrWhiteSpace(request.Status) &&
            System.Enum.TryParse<RiverLi.Blog.Services.Blog.Domain.Enum.ArticleStatus>(request.Status, true, out var status))
            query = query.Where(a => a.Status == status);

        // 8. 先统计总数 (在分页之前)
        var totalCount = await query.CountAsync(cancellationToken);

        // 9. 排序策略：默认按创建时间倒序
        query = request.SortBy?.ToLower() switch
        {
            "viewcount" => query.OrderByDescending(a => a.ViewCount),
            "created" => query.OrderByDescending(a => a.CreateTime),
            _ => query.OrderByDescending(a => a.CreateTime)
        };

        // 10. 分页 + 投影到 DTO (仅 select 需要的字段，避免 select * 性能损耗)
        var items = await query
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new ArticleDto(
                a.Id, a.Title, a.Slug, a.Summary, a.CoverUrl, a.AuthorName,
                a.Status.ToString(), a.Category != null ? a.Category.Name : null,
                a.Tags.Select(t => t.Tag!.Name).ToList(),
                a.ViewCount, a.Comments.Count, a.CreateTime, a.ScheduledPublishTime
            ))
            .ToListAsync(cancellationToken);

        // 11. 组装分页结果
        var result = PagedResult<ArticleDto>.SuccessResult(items, totalCount, request.PageIndex, request.PageSize);

        // 12. TODO: 缓存暂时关闭
        // _cache.Set(cacheKey, result, TimeSpan.FromMinutes(0.5));

        return result;
    }
}