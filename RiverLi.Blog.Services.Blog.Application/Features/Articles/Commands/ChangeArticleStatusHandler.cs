using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

public class ChangeArticleStatusHandler : IRequestHandler<ChangeArticleStatusCommand, Result>
{
    private readonly IRepository<Article, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public ChangeArticleStatusHandler(IRepository<Article, Guid> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ChangeArticleStatusCommand request, CancellationToken cancellationToken)
    {
        // 系统内部调用（定时任务等）跳过用户校验
        if (!request.IsSystem)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
                return Result.FailResult("未登录用户无法操作");
        }

        var article = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (article == null)
            return Result.FailResult("文章不存在");

        // 仅登录用户做作者权限校验，系统调用不做
        if (!request.IsSystem && article.AuthorId != _currentUser.Id.ToString())
            return Result.FailResult("只能操作自己的文章");

        article.ChangeStatus(request.Status);
        await _repository.UpdateAsync(article, cancellationToken);
        var saved = await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        if (!saved)
            return Result.FailResult("状态更新失败，请重试");
        return Result.SuccessResult();
    }
}
