using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using Microsoft.EntityFrameworkCore;

namespace RiverLi.Blog.Services.Blog.Application.Queries
{
    public record GetLinkQuery(Guid Id) : IRequest<Result<LinkDto>>;
    public record GetLinkListQuery : IRequest<Result<List<LinkDto>>>;
    public class GetLinkPagingQuery : PagedQuery, IRequest<PagedResult<LinkDto>>;
    public record GetLinkTypesQuery : IRequest<Result<List<LinkTypeDto>>>;

    public class LinkQueryHandler :
        IRequestHandler<GetLinkQuery, Result<LinkDto>>,
        IRequestHandler<GetLinkListQuery, Result<List<LinkDto>>>,
        IRequestHandler<GetLinkPagingQuery, PagedResult<LinkDto>>,
        IRequestHandler<GetLinkTypesQuery, Result<List<LinkTypeDto>>>
    {
        private readonly IRepository<Link, Guid> _linkRepo;
        private readonly IRepository<LinkType, Guid> _typeRepo;

        public LinkQueryHandler(IRepository<Link, Guid> linkRepo, IRepository<LinkType, Guid> typeRepo)
        {
            _linkRepo = linkRepo;
            _typeRepo = typeRepo;
        }

        public async Task<Result<LinkDto>> Handle(GetLinkQuery request, CancellationToken ct)
        {
            var link = await _linkRepo.GetByIdAsync(request.Id, ct);
            if (link == null) return Result<LinkDto>.FailResult("友链不存在", 404);
            return Result<LinkDto>.SuccessResult(MapLink(link));
        }

        public async Task<Result<List<LinkDto>>> Handle(GetLinkListQuery request, CancellationToken ct)
        {
            var list = await _linkRepo.AsQueryable()
                .Where(l => l.Status == "approved")
                .OrderByDescending(l => l.CreateTime).ToListAsync(ct);
            return Result<List<LinkDto>>.SuccessResult(list.Select(MapLink).ToList());
        }

        public async Task<PagedResult<LinkDto>> Handle(GetLinkPagingQuery request, CancellationToken ct)
        {
            var query = _linkRepo.AsQueryable().OrderByDescending(l => l.CreateTime);
            var total = await query.CountAsync(ct);
            var items = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize).ToListAsync(ct);
            return PagedResult<LinkDto>.SuccessResult(
                items.Select(MapLink).ToList(), total, request.PageIndex, request.PageSize);
        }

        public async Task<Result<List<LinkTypeDto>>> Handle(GetLinkTypesQuery request, CancellationToken ct)
        {
            var list = await _typeRepo.GetAllAsync(ct);
            return Result<List<LinkTypeDto>>.SuccessResult(list.Select(MapType).ToList());
        }

        private static LinkDto MapLink(Link l) => new(l.Id, l.Name, l.Url, l.Logo,
            l.Description, l.TypeId, l.Status, l.Email, l.CreateTime);
        private static LinkTypeDto MapType(LinkType t) => new(t.Id, t.Name, t.Icon, t.CreateTime);
    }
}
