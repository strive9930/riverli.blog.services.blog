using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.FriendLinks.Commands;

public class CreateFriendLinkHandler : IRequestHandler<CreateFriendLinkCommand, Result<Guid>>
{
    private readonly IRepository<FriendLink, Guid> _repository;

    public CreateFriendLinkHandler(IRepository<FriendLink, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle(CreateFriendLinkCommand request, CancellationToken cancellationToken)
    {
        var link = new FriendLink(
            request.SiteName, request.SiteDescription, request.Url,
            request.AvatarUrl, request.Owner, request.RssUrl);

        await _repository.AddAsync(link, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Result<Guid>.SuccessResult(link.Id);
    }
}
