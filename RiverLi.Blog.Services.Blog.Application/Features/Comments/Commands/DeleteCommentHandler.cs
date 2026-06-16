using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Comments.Commands;

public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand, Result>
{
    private readonly IRepository<Comment, Guid> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCommentHandler(IRepository<Comment, Guid> repository, ICurrentUser currentUser, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Result.FailResult("未登录用户无法删除评论");

        var comment = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (comment == null)
            return Result.FailResult("评论不存在");

        await _repository.DeleteAsync(comment, cancellationToken);
        var saved = await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        if (!saved)
            return Result.FailResult("删除失败，请重试");
        return Result.SuccessResult();
    }
}
