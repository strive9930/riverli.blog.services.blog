using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, Result>
{
    private readonly IRepository<Comment, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public DeleteCommentHandler(IRepository<Comment, Guid> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Result.FailResult("未登录用户无法删除评论");

        var comment = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (comment == null)
            return Result.FailResult("评论不存在");

        // 彻底删除 (物理删除)
        await _repository.DeleteAsync(comment, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Result.SuccessResult();
    }
}
