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

namespace RiverLi.Blog.Services.Blog.Application.Features.Tags.Queries;

public class GetTagOptionsHandler : IRequestHandler<GetTagOptionsQuery, Result<List<TagOptionDto>>>
{
    private readonly IRepository<Tag, Guid> _tagRepo;
    private readonly IMemoryCache _cache;

    public GetTagOptionsHandler(IRepository<Tag, Guid> tagRepo, IMemoryCache cache)
    {
        _tagRepo = tagRepo;
        _cache = cache;
    }

    public async Task<Result<List<TagOptionDto>>> Handle(GetTagOptionsQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "tag_options";

        // TODO: 缓存暂时关闭
        // if (_cache.TryGetValue<List<TagOptionDto>>(cacheKey, out var cached))
        //     return Result<List<TagOptionDto>>.SuccessResult(cached);

        var tags = await _tagRepo
            .AsQueryable()
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.Name)
            .Select(t => new TagOptionDto(t.Id, t.Name))
            .ToListAsync(cancellationToken);

        // _cache.Set(cacheKey, tags, TimeSpan.FromMinutes(3));
        return Result<List<TagOptionDto>>.SuccessResult(tags);
    }
}