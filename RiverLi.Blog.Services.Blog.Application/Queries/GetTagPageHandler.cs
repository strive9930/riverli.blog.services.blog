using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.Blog.Services.Blog.Infrastructure.Data;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Queries;

public class GetTagPageHandler : IRequestHandler<GetTagPageQuery, Result<PagedResult<TagDto>>>
{
    private readonly BlogDbContext _dbContext;

    public GetTagPageHandler(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<TagDto>>> Handle(GetTagPageQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Tags
            .AsNoTracking()
            .Where(t => !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            query = query.Where(t => t.Name.Contains(request.Keyword));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(t => t.Name)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TagDto(
                t.Id,
                t.Name,
                t.Slug,
                _dbContext.ArticleTags.Count(at => at.TagId == t.Id)
            ))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<TagDto>>.SuccessResult(
            PagedResult<TagDto>.SuccessResult(items, totalCount, request.PageIndex, request.PageSize));
    }
}
