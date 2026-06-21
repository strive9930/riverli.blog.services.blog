using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

public class IncrementArticleViewCountHandler : IRequestHandler<IncrementArticleViewCountCommand, Result>
{
    private readonly IRepository<Article, Guid> _repository;

    public IncrementArticleViewCountHandler(IRepository<Article, Guid> repository) => _repository = repository;

    public async Task<Result> Handle(IncrementArticleViewCountCommand request, CancellationToken cancellationToken)
    {
        var article = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (article == null) return Result.FailResult("文章不存在");

        article.IncrementViewCount();
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Result.SuccessResult();
    }
}
