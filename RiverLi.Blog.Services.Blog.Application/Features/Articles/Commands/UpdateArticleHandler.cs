using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Domain.Enum;
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

        var slug = !string.IsNullOrWhiteSpace(request.Slug) 
            ? request.Slug 
            : SlugHelper.Generate(request.Title);

        article.Update(request.Title, slug, request.Content, request.Summary, request.CoverUrl, request.CategoryId);

        if (request.TagIds != null)
            article.SetTags(request.TagIds);

        // 定时发布逻辑
        if (request.ScheduledPublishTime.HasValue)
            article.SchedulePublish(request.ScheduledPublishTime.Value);
        else if (article.Status == ArticleStatus.Scheduled)
            article.CancelScheduledPublish(); // 清空定时时间则取消定时

        await _repository.UpdateAsync(article, cancellationToken);
        var saved = await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        if (!saved)
            return Result.FailResult("文章保存失败，请重试");

        return Result.SuccessResult();
    }
}
