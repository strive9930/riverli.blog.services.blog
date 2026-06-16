using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

public class UpdateArticleHandler : IRequestHandler<UpdateArticleCommand, Result>
{
    private readonly IRepository<Article, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public UpdateArticleHandler(IRepository<Article, Guid> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
            return Result.FailResult("未登录用户无法修改文章");

        var article = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (article == null)
            return Result.FailResult("文章不存在");

        if (article.AuthorId != _currentUser.Id.ToString())
            return Result.FailResult("只能修改自己的文章");

        article.Update(request.Title, request.Content, request.Summary, request.CoverUrl, request.CategoryId);

        if (request.TagIds != null)
            article.SetTags(request.TagIds);

        await _repository.UpdateAsync(article, cancellationToken);
        var saved = await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        if (!saved)
            return Result.FailResult("文章保存失败，请重试");

        return Result.SuccessResult();
    }
}
