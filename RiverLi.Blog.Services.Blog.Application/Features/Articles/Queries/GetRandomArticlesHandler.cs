using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

public class GetRandomArticlesHandler : IRequestHandler<GetRandomArticlesQuery, Result<List<ArticleDto>>>
{
    private readonly IRepository<Article, Guid> _repository;

    public GetRandomArticlesHandler(IRepository<Article, Guid> repository) => _repository = repository;

    public async Task<Result<List<ArticleDto>>> Handle(GetRandomArticlesQuery request, CancellationToken cancellationToken)
    {
        var totalPublished = await _repository.AsQueryable()
            .Where(a => !a.IsDeleted && a.Status == ArticleStatus.Published)
            .CountAsync(cancellationToken);

        if (totalPublished == 0)
            return Result<List<ArticleDto>>.SuccessResult(new List<ArticleDto>());

        var skip = Math.Max(0, Random.Shared.Next(totalPublished) - request.Count);
        var take = Math.Min(request.Count, totalPublished);

        var items = await _repository.AsQueryable()
            .Where(a => !a.IsDeleted && a.Status == ArticleStatus.Published)
            .OrderBy(a => a.CreateTime)
            .Skip(skip)
            .Take(take)
            .Select(a => new ArticleDto(
                a.Id, a.Title, a.Slug, a.Summary, a.CoverUrl, a.AuthorName,
                a.Status.ToString(), a.Category != null ? a.Category.Name : null,
                a.Tags.Select(t => t.Tag!.Name).ToList(),
                a.ViewCount, a.Comments.Count, a.CreateTime, a.ScheduledPublishTime
            ))
            .ToListAsync(cancellationToken);

        // 随机打乱
        items = items.OrderBy(_ => Random.Shared.Next()).ToList();

        return Result<List<ArticleDto>>.SuccessResult(items);
    }
}
