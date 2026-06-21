using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.FriendLinks.Commands;

public class DeleteFriendLinkHandler : IRequestHandler<DeleteFriendLinkCommand, Result>
{
    private readonly IRepository<FriendLink, Guid> _repository;

    public DeleteFriendLinkHandler(IRepository<FriendLink, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(DeleteFriendLinkCommand request, CancellationToken cancellationToken)
    {
        var link = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (link == null) return Result.FailResult("友链不存在");

        await _repository.DeleteAsync(link, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Result.SuccessResult();
    }
}
