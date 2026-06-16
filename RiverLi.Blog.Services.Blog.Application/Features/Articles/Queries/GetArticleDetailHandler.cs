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

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

public class GetArticleDetailHandler : IRequestHandler<GetArticleDetailQuery, Result<ArticleDetailDto>>
{
    private readonly IRepository<Article, Guid> _repository;
    private readonly IMemoryCache _cache;

    public GetArticleDetailHandler(IRepository<Article, Guid> repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<Result<ArticleDetailDto>> Handle(GetArticleDetailQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"article_detail_{request.Id}";

        if (_cache.TryGetValue<ArticleDetailDto>(cacheKey, out var cached))
            return Result<ArticleDetailDto>.SuccessResult(cached);

        var article = await _repository
            .AsQueryable()
            .Where(a => !a.IsDeleted && a.Id == request.Id)
            .Select(a => new ArticleDetailDto(
                a.Id, a.Title, a.Content, a.Summary, a.CoverUrl,
                a.AuthorId, a.AuthorName, a.Status.ToString(),
                a.CategoryId, null,
                a.Tags.Select(t => new TagDto(t.TagId, "", "", 0)).ToList(),
                a.ViewCount, a.Comments.Count, a.CreateTime, a.UpdateTime
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (article == null)
            return Result<ArticleDetailDto>.FailResult("���²�����");

        _cache.Set(cacheKey, article, TimeSpan.FromMinutes(3));
        return Result<ArticleDetailDto>.SuccessResult(article);
    }
}
