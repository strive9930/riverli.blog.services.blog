using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

public class GetScheduledArticlesHandler
    : IRequestHandler<GetScheduledArticlesQuery, Result<List<Guid>>>
{
    private readonly IRepository<Article, Guid> _repository;

    public GetScheduledArticlesHandler(IRepository<Article, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<Guid>>> Handle(
        GetScheduledArticlesQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;
        var ids = await _repository.AsQueryable()
            .Where(a => a.Status == ArticleStatus.Scheduled
                     && a.ScheduledPublishTime <= now)
            .Select(a => a.Id)
            .ToListAsync(cancellationToken);

        return Result<List<Guid>>.SuccessResult(ids);
    }
}
