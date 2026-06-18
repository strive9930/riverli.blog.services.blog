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

namespace RiverLi.Blog.Services.Blog.Application.Features.Medias.Queries;

public class GetMediaPageHandler : IRequestHandler<GetMediaPageQuery, PagedResult<MediaDto>>
{
    private readonly IRepository<Media, Guid> _repository;
    private readonly IMemoryCache _cache;

    public GetMediaPageHandler(IRepository<Media, Guid> repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<PagedResult<MediaDto>> Handle(GetMediaPageQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"media_page_{request.PageIndex}_{request.PageSize}_{request.Keyword}_{request.ContentType}";

        if (_cache.TryGetValue<PagedResult<MediaDto>>(cacheKey, out var cached))
            return cached;

        var query = _repository.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
            query = query.Where(m => m.FileName.Contains(request.Keyword));

        if (!string.IsNullOrWhiteSpace(request.ContentType))
            query = query.Where(m => m.ContentType.StartsWith(request.ContentType));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(m => m.CreateTime)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MediaDto(m.Id, m.FileName, m.Url, m.ContentType, m.FileSize, m.UploadedBy, m.CreateTime))
            .ToListAsync(cancellationToken);

        var result = PagedResult<MediaDto>.SuccessResult(items, totalCount, request.PageIndex, request.PageSize);
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));

        return result;
    }
}
