using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.Blog.Services.Blog.Infrastructure.Data;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Queries;

public class GetArticleDetailHandler : IRequestHandler<GetArticleDetailQuery, Result<ArticleDetailDto>>
{
    private readonly BlogDbContext _dbContext;

    public GetArticleDetailHandler(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<ArticleDetailDto>> Handle(GetArticleDetailQuery request, CancellationToken cancellationToken)
    {
        var article = await _dbContext.Articles
            .AsNoTracking()
            .Include(a => a.Tags)
            .Where(a => !a.IsDeleted && a.Id == request.Id)
            .Select(a => new ArticleDetailDto(
                a.Id,
                a.Title,
                a.Content,
                a.Summary,
                a.CoverUrl,
                a.AuthorId,
                a.AuthorName,
                a.Status.ToString(),
                a.CategoryId,
                null, // CategoryName — 需要 JOIN
                a.Tags.Select(t => new TagDto(t.TagId, "", "", 0)).ToList(), // TagName 需要二次查询
                a.ViewCount,
                a.Comments.Count,
                a.CreateTime,
                a.UpdateTime
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (article == null)
            return Result<ArticleDetailDto>.FailResult("文章不存在");

        return Result<ArticleDetailDto>.SuccessResult(article);
    }
}
