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

namespace RiverLi.Blog.Services.Blog.Application.Features.Categories.Queries;

public class GetCategoryOptionsHandler : IRequestHandler<GetCategoryOptionsQuery, Result<List<CategoryOptionDto>>>
{
    private readonly IRepository<Category, Guid> _categoryRepo;
    private readonly IRepository<Article, Guid> _articleRepo;
    private readonly IMemoryCache _cache;

    public GetCategoryOptionsHandler(
        IRepository<Category, Guid> categoryRepo,
        IRepository<Article, Guid> articleRepo,
        IMemoryCache cache)
    {
        _categoryRepo = categoryRepo;
        _articleRepo = articleRepo;
        _cache = cache;
    }

    public async Task<Result<List<CategoryOptionDto>>> Handle(GetCategoryOptionsQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "category_options";

        // TODO: 缓存暂时关闭
        // if (_cache.TryGetValue<List<CategoryOptionDto>>(cacheKey, out var cached))
        //     return Result<List<CategoryOptionDto>>.SuccessResult(cached);

        var categories = await _categoryRepo
            .AsQueryable()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .Select(c => new CategoryFlatItem
            {
                Id = c.Id, Name = c.Name, ParentId = c.ParentId,
                SortOrder = c.SortOrder,
                ArticleCount = _articleRepo.AsQueryable().Count(a => a.CategoryId == c.Id && !a.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        var result = new List<CategoryOptionDto>();
        Flatten(categories, null, 0, result);

        // _cache.Set(cacheKey, result, TimeSpan.FromMinutes(3));
        return Result<List<CategoryOptionDto>>.SuccessResult(result);
    }

    private static void Flatten(List<CategoryFlatItem> all, Guid? parentId, int depth, List<CategoryOptionDto> result)
    {
        var children = all.Where(c => c.ParentId == parentId).ToList();
        foreach (var cat in children)
        {
            result.Add(new CategoryOptionDto(cat.Id, cat.Name, cat.ParentId, cat.SortOrder, depth, cat.ArticleCount));
            Flatten(all, cat.Id, depth + 1, result);
        }
    }

    private class CategoryFlatItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public int SortOrder { get; set; }
        public int ArticleCount { get; set; }
    }
}
