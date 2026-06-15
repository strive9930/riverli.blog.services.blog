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

public class GetCommentPageHandler : IRequestHandler<GetCommentPageQuery, Result<PagedResult<CommentDto>>>
{
    private readonly IRepository<Comment, Guid> _commentRepo;
    private readonly IRepository<Article, Guid> _articleRepo;
    private readonly IMemoryCache _cache;

    public GetCommentPageHandler(
        IRepository<Comment, Guid> commentRepo,
        IRepository<Article, Guid> articleRepo,
        IMemoryCache cache)
    {
        _commentRepo = commentRepo;
        _articleRepo = articleRepo;
        _cache = cache;
    }

    public async Task<Result<PagedResult<CommentDto>>> Handle(GetCommentPageQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"comments_page_{request.PageIndex}_{request.PageSize}_{request.Status}_{request.ArticleId}";

        if (_cache.TryGetValue<PagedResult<CommentDto>>(cacheKey, out var cached))
            return Result<PagedResult<CommentDto>>.SuccessResult(cached);

        var query = _commentRepo.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            System.Enum.TryParse<RiverLi.Blog.Services.Blog.Domain.Enum.CommentStatus>(request.Status, true, out var status))
            query = query.Where(c => c.Status == status);

        if (request.ArticleId.HasValue)
            query = query.Where(c => c.ArticleId == request.ArticleId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreateTime)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CommentDto(
                c.Id, c.ArticleId,
                _articleRepo.AsQueryable().Where(a => a.Id == c.ArticleId).Select(a => a.Title).FirstOrDefault() ?? "",
                c.ReviewerName, c.Content, c.Status.ToString(), c.CreateTime
            ))
            .ToListAsync(cancellationToken);

        var result = PagedResult<CommentDto>.SuccessResult(items, totalCount, request.PageIndex, request.PageSize);
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));

        return Result<PagedResult<CommentDto>>.SuccessResult(result);
    }
}
