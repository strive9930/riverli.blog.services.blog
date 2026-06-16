using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

public class CreateArticleHandler : IRequestHandler<CreateArticleCommand, Result<Guid>>
{
    private readonly IRepository<Article, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public CreateArticleHandler(
        IRepository<Article, Guid> repository, 
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
            return Result<Guid>.FailResult("未登录用户无法发布文章");

        var authorId = _currentUser.Id.ToString()!; 
        var authorName = _currentUser.UserName ?? "匿名创作者";

        if (string.IsNullOrWhiteSpace(request.Content))
            return Result<Guid>.FailResult("文章正文内容不能为空");

        var article = new Article(
            title: request.Title,
            content: request.Content,
            summary: request.Summary,
            coverUrl: request.CoverUrl,
            categoryId: request.CategoryId,
            authorId: authorId,
            authorName: authorName
        );

        if (request.TagIds != null && request.TagIds.Any())
            article.SetTags(request.TagIds);

        await _repository.AddAsync(article, cancellationToken);
        var saved = await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        if (!saved)
            return Result<Guid>.FailResult("文章保存失败，请重试");

        return Result<Guid>.SuccessResult(article.Id);
    }
}
