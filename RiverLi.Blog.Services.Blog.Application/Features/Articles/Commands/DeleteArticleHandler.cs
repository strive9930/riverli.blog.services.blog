using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

public class DeleteArticleHandler : IRequestHandler<DeleteArticleCommand, Result>
{
    private readonly IRepository<Article, Guid> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteArticleHandler(IRepository<Article, Guid> repository, ICurrentUser currentUser, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
            return Result.FailResult("未登录用户无法删除文章");

        var article = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (article == null)
            return Result.FailResult("文章不存在");

        if (article.AuthorId != _currentUser.Id.ToString())
            return Result.FailResult("只能删除自己的文章");

        article.MarkAsDeleted();
        var saved = await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        if (!saved)
            return Result.FailResult("删除失败，请重试");
        return Result.SuccessResult();
    }
}
