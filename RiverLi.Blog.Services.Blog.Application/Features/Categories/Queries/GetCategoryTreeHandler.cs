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

public class GetCategoryTreeHandler : IRequestHandler<GetCategoryTreeQuery, Result<List<CategoryDto>>>
{
    private readonly IRepository<Category, Guid> _categoryRepo;
    private readonly IRepository<Article, Guid> _articleRepo;
    private readonly IMemoryCache _cache;

    public GetCategoryTreeHandler(
        IRepository<Category, Guid> categoryRepo,
        IRepository<Article, Guid> articleRepo,
        IMemoryCache cache)
    {
        _categoryRepo = categoryRepo;
        _articleRepo = articleRepo;
        _cache = cache;
    }

    public async Task<Result<List<CategoryDto>>> Handle(GetCategoryTreeQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "category_tree";

        if (_cache.TryGetValue<List<CategoryDto>>(cacheKey, out var cached))
            return Result<List<CategoryDto>>.SuccessResult(cached);

        var categories = await _categoryRepo
            .AsQueryable()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.SortOrder)
            .Select(c => new
            {
                c.Id, c.Name, c.Slug, c.Description, c.SortOrder, c.ParentId,
                ArticleCount = _articleRepo.AsQueryable().Count(a => a.CategoryId == c.Id && !a.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        var dtoMap = categories.ToDictionary(
            c => c.Id,
            c => new CategoryDto(c.Id, c.Name, c.Slug, c.Description, c.SortOrder, c.ArticleCount, new List<CategoryDto>())
        );

        var roots = new List<CategoryDto>();
        foreach (var cat in categories)
        {
            var dto = dtoMap[cat.Id];
            if (cat.ParentId.HasValue && dtoMap.ContainsKey(cat.ParentId.Value))
                dtoMap[cat.ParentId.Value].Children.Add(dto);
            else
                roots.Add(dto);
        }

        _cache.Set(cacheKey, roots, TimeSpan.FromMinutes(3));
        return Result<List<CategoryDto>>.SuccessResult(roots);
    }
}
