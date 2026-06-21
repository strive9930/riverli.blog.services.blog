using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Records.Queries;

public record GetRecordPageQuery(int PageIndex = 1, int PageSize = 20, bool AdminView = false) : IRequest<PagedResult<RecordDto>>;

public class GetRecordPageHandler : IRequestHandler<GetRecordPageQuery, PagedResult<RecordDto>>
{
    private readonly IRepository<Record, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public GetRecordPageHandler(IRepository<Record, Guid> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<RecordDto>> Handle(GetRecordPageQuery request, CancellationToken cancellationToken)
    {
        var query = _repository.AsQueryable();
        var isAdmin = _currentUser.IsAuthenticated && _currentUser.IsInRole("Admin");

        if (!request.AdminView && !isAdmin)
            query = query.Where(r => r.IsPublic);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.CreateTime)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RecordDto(r.Id, r.Content, r.ImageUrls, r.Location, r.IsPublic, r.CreateTime))
            .ToListAsync(cancellationToken);

        return PagedResult<RecordDto>.SuccessResult(items, totalCount, request.PageIndex, request.PageSize);
    }
}
