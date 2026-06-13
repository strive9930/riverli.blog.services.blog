using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public class CreateCommentHandler : IRequestHandler<CreateCommentCommand, Result<Guid>>
{
    private readonly IRepository<Comment, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public CreateCommentHandler(IRepository<Comment, Guid> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
            return Result<Guid>.FailResult("请先登录后再评论");

        if (string.IsNullOrWhiteSpace(request.Content))
            return Result<Guid>.FailResult("评论内容不能为空");

        var reviewerId = _currentUser.Id.ToString()!;
        var reviewerName = _currentUser.UserName ?? "匿名用户";

        var comment = new Comment(request.ArticleId, reviewerId, reviewerName, request.Content);
        await _repository.AddAsync(comment, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Result<Guid>.SuccessResult(comment.Id);
    }
}
