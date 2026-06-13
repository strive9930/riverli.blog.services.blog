using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public class AuditCommentHandler : IRequestHandler<AuditCommentCommand, Result>
{
    private readonly IRepository<Comment, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public AuditCommentHandler(IRepository<Comment, Guid> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(AuditCommentCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Result.FailResult("未登录用户无法审核评论");

        var comment = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (comment == null)
            return Result.FailResult("评论不存在");

        comment.Audit(request.Status);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Result.SuccessResult();
    }
}
