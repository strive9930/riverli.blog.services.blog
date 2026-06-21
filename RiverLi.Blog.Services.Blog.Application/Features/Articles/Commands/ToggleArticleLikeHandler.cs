using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

public class ToggleArticleLikeHandler : IRequestHandler<ToggleArticleLikeCommand, Result<bool>>
{
    private readonly IRepository<Article, Guid> _articleRepo;
    private readonly IRepository<ArticleLike, Guid> _likeRepo;

    public ToggleArticleLikeHandler(IRepository<Article, Guid> articleRepo, IRepository<ArticleLike, Guid> likeRepo)
    {
        _articleRepo = articleRepo;
        _likeRepo = likeRepo;
    }

    public async Task<Result<bool>> Handle(ToggleArticleLikeCommand request, CancellationToken cancellationToken)
    {
        var article = await _articleRepo.GetByIdAsync(request.ArticleId, cancellationToken);
        if (article == null) return Result<bool>.FailResult("文章不存在");

        var existing = await _likeRepo.AsQueryable()
            .FirstOrDefaultAsync(l => l.ArticleId == request.ArticleId && l.UserId == request.UserId, cancellationToken);

        if (existing != null)
        {
            await _likeRepo.DeleteAsync(existing, cancellationToken);
            await _likeRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            return Result<bool>.SuccessResult(false); // 取消点赞
        }

        var like = new ArticleLike(request.ArticleId, request.UserId);
        await _likeRepo.AddAsync(like, cancellationToken);
        await _likeRepo.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Result<bool>.SuccessResult(true); // 点赞成功
    }
}
