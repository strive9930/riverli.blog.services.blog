using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.Blog.Services.Blog.Infrastructure.Data;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Queries;

public class GetCommentPageHandler : IRequestHandler<GetCommentPageQuery, Result<PagedResult<CommentDto>>>
{
    private readonly BlogDbContext _dbContext;

    public GetCommentPageHandler(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<PagedResult<CommentDto>>> Handle(GetCommentPageQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Comments.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            System.Enum.TryParse<RiverLi.Blog.Services.Blog.Domain.Enum.CommentStatus>(request.Status, true, out var status))
        {
            query = query.Where(c => c.Status == status);
        }

        if (request.ArticleId.HasValue)
        {
            query = query.Where(c => c.ArticleId == request.ArticleId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreateTime)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CommentDto(
                c.Id,
                c.ArticleId,
                _dbContext.Articles.Where(a => a.Id == c.ArticleId).Select(a => a.Title).FirstOrDefault() ?? "",
                c.ReviewerName,
                c.Content,
                c.Status.ToString(),
                c.CreateTime
            ))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<CommentDto>>.SuccessResult(
            PagedResult<CommentDto>.SuccessResult(items, totalCount, request.PageIndex, request.PageSize));
    }
}
