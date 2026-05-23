using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using Microsoft.EntityFrameworkCore;

namespace RiverLi.Blog.Services.Blog.Application.Queries
{
    public record GetWallQuery(Guid Id) : IRequest<Result<WallDto>>;
    public record GetWallListQuery : IRequest<Result<List<WallDto>>>;
    public class GetWallPagingQuery : PagedQuery, IRequest<PagedResult<WallDto>>;
    public record GetWallCatesQuery : IRequest<Result<List<WallCateDto>>>;

    public class WallQueryHandler :
        IRequestHandler<GetWallQuery, Result<WallDto>>,
        IRequestHandler<GetWallListQuery, Result<List<WallDto>>>,
        IRequestHandler<GetWallPagingQuery, PagedResult<WallDto>>,
        IRequestHandler<GetWallCatesQuery, Result<List<WallCateDto>>>
    {
        private readonly IRepository<Wall, Guid> _wallRepo;
        private readonly IRepository<WallCate, Guid> _cateRepo;

        public WallQueryHandler(IRepository<Wall, Guid> wallRepo, IRepository<WallCate, Guid> cateRepo)
        {
            _wallRepo = wallRepo;
            _cateRepo = cateRepo;
        }

        public async Task<Result<WallDto>> Handle(GetWallQuery request, CancellationToken ct)
        {
            var wall = await _wallRepo.GetByIdAsync(request.Id, ct);
            if (wall == null) return Result<WallDto>.FailResult("留言不存在", 404);
            return Result<WallDto>.SuccessResult(MapWall(wall));
        }

        public async Task<Result<List<WallDto>>> Handle(GetWallListQuery request, CancellationToken ct)
        {
            var list = await _wallRepo.AsQueryable()
                .Where(w => w.Status == "approved")
                .OrderByDescending(w => w.CreateTime).ToListAsync(ct);
            return Result<List<WallDto>>.SuccessResult(list.Select(MapWall).ToList());
        }

        public async Task<PagedResult<WallDto>> Handle(GetWallPagingQuery request, CancellationToken ct)
        {
            var query = _wallRepo.AsQueryable().OrderByDescending(w => w.CreateTime);
            var total = await query.CountAsync(ct);
            var items = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize).ToListAsync(ct);
            return PagedResult<WallDto>.SuccessResult(
                items.Select(MapWall).ToList(), total, request.PageIndex, request.PageSize);
        }

        public async Task<Result<List<WallCateDto>>> Handle(GetWallCatesQuery request, CancellationToken ct)
        {
            var list = await _cateRepo.GetAllAsync(ct);
            return Result<List<WallCateDto>>.SuccessResult(list.Select(MapCate).ToList());
        }

        private static WallDto MapWall(Wall w) => new(w.Id, w.Content, w.Email, w.Name,
            w.Url, w.Avatar, w.CateId, w.Status, w.CreateTime);

        private static WallCateDto MapCate(WallCate c) => new(c.Id, c.Name, c.Icon, c.Color, c.CreateTime);
    }
}
