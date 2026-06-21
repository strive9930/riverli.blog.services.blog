using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.FriendLinks.Queries;

public class GetFriendLinkPageHandler : IRequestHandler<GetFriendLinkPageQuery, PagedResult<FriendLinkDto>>
{
    private readonly IRepository<FriendLink, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public GetFriendLinkPageHandler(IRepository<FriendLink, Guid> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<FriendLinkDto>> Handle(GetFriendLinkPageQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.AsQueryable();
        var isAdmin = _currentUser.IsAuthenticated && _currentUser.IsInRole("Admin");

        if (!request.AdminView && !isAdmin)
            query = query.Where(f => f.Status == FriendLinkStatus.Approved);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(f => f.IsTop)
            .ThenBy(f => f.SortOrder)
            .ThenByDescending(f => f.CreateTime)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new FriendLinkDto(
                f.Id, f.SiteName, f.SiteDescription, f.Url,
                f.AvatarUrl, f.Owner, f.RssUrl,
                f.Status.ToString(), f.SortOrder, f.IsTop, f.CreateTime
            ))
            .ToListAsync(cancellationToken);

        return PagedResult<FriendLinkDto>.SuccessResult(items, totalCount, request.PageIndex, request.PageSize);
    }
}
