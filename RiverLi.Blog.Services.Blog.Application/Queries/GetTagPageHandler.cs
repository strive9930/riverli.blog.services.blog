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

namespace RiverLi.Blog.Services.Blog.Application.Queries;

public class GetTagPageHandler : IRequestHandler<GetTagPageQuery, Result<PagedResult<TagDto>>>
{
    private readonly IRepository<Tag, Guid> _tagRepo;
    private readonly IRepository<ArticleTag, Guid> _articleTagRepo;
    private readonly IMemoryCache _cache;

    public GetTagPageHandler(
        IRepository<Tag, Guid> tagRepo,
        IRepository<ArticleTag, Guid> articleTagRepo,
        IMemoryCache cache)
    {
        _tagRepo = tagRepo;
        _articleTagRepo = articleTagRepo;
        _cache = cache;
    }

    public async Task<Result<PagedResult<TagDto>>> Handle(GetTagPageQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"tags_page_{request.PageIndex}_{request.PageSize}_{request.Keyword}";

        if (_cache.TryGetValue<PagedResult<TagDto>>(cacheKey, out var cached))
            return Result<PagedResult<TagDto>>.SuccessResult(cached);

        var query = _tagRepo.AsQueryable().Where(t => !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Keyword))
            query = query.Where(t => t.Name.Contains(request.Keyword));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(t => t.Name)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TagDto(
                t.Id, t.Name, t.Slug,
                _articleTagRepo.AsQueryable().Count(at => at.TagId == t.Id)
            ))
            .ToListAsync(cancellationToken);

        var result = PagedResult<TagDto>.SuccessResult(items, totalCount, request.PageIndex, request.PageSize);
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));

        return Result<PagedResult<TagDto>>.SuccessResult(result);
    }
}
