using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

public class GetArticleLikeCountHandler : IRequestHandler<GetArticleLikeCountQuery, Result<int>>
{
    private readonly IRepository<ArticleLike, Guid> _repository;

    public GetArticleLikeCountHandler(IRepository<ArticleLike, Guid> repository) => _repository = repository;

    public async Task<Result<int>> Handle(GetArticleLikeCountQuery request, CancellationToken cancellationToken)
    {
        var count = await _repository.AsQueryable()
            .Where(l => l.ArticleId == request.ArticleId)
            .CountAsync(cancellationToken);

        return Result<int>.SuccessResult(count);
    }
}

public class CheckUserLikedHandler : IRequestHandler<CheckUserLikedQuery, Result<bool>>
{
    private readonly IRepository<ArticleLike, Guid> _repository;

    public CheckUserLikedHandler(IRepository<ArticleLike, Guid> repository) => _repository = repository;

    public async Task<Result<bool>> Handle(CheckUserLikedQuery request, CancellationToken cancellationToken)
    {
        var liked = await _repository.AsQueryable()
            .AnyAsync(l => l.ArticleId == request.ArticleId && l.UserId == request.UserId, cancellationToken);

        return Result<bool>.SuccessResult(liked);
    }
}
