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

namespace RiverLi.Blog.Services.Blog.Application.Features.Messages.Queries;

public record GetMessagePageQuery(int PageIndex = 1, int PageSize = 20, bool AdminView = false) : IRequest<PagedResult<MessageDto>>;

public class GetMessagePageHandler : IRequestHandler<GetMessagePageQuery, PagedResult<MessageDto>>
{
    private readonly IRepository<Message, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public GetMessagePageHandler(IRepository<Message, Guid> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<MessageDto>> Handle(GetMessagePageQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.AsQueryable();
        var isAdmin = _currentUser.IsAuthenticated && _currentUser.IsInRole("Admin");

        if (!request.AdminView && !isAdmin)
            query = query.Where(m => m.Status == CommentStatus.Approved);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(m => m.CreateTime)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MessageDto(m.Id, m.Nickname, m.Content, m.Contact, m.Status.ToString(), m.CreateTime))
            .ToListAsync(cancellationToken);

        return PagedResult<MessageDto>.SuccessResult(items, totalCount, request.PageIndex, request.PageSize);
    }
}
