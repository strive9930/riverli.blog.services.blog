using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using Microsoft.EntityFrameworkCore;

namespace RiverLi.Blog.Services.Blog.Application.Queries
{
    public record GetAlbumCateQuery(Guid Id) : IRequest<Result<AlbumCateDto>>;
    public record GetAlbumCateListQuery : IRequest<Result<List<AlbumCateDto>>>;
    public record GetAlbumImageQuery(Guid Id) : IRequest<Result<AlbumImageDto>>;
    public record GetAlbumImageListQuery(int CateId) : IRequest<Result<List<AlbumImageDto>>>;

    public class AlbumQueryHandler :
        IRequestHandler<GetAlbumCateQuery, Result<AlbumCateDto>>,
        IRequestHandler<GetAlbumCateListQuery, Result<List<AlbumCateDto>>>,
        IRequestHandler<GetAlbumImageQuery, Result<AlbumImageDto>>,
        IRequestHandler<GetAlbumImageListQuery, Result<List<AlbumImageDto>>>
    {
        private readonly IRepository<AlbumCate, Guid> _cateRepo;
        private readonly IRepository<AlbumImage, Guid> _imageRepo;

        public AlbumQueryHandler(IRepository<AlbumCate, Guid> cateRepo, IRepository<AlbumImage, Guid> imageRepo)
        {
            _cateRepo = cateRepo;
            _imageRepo = imageRepo;
        }

        public async Task<Result<AlbumCateDto>> Handle(GetAlbumCateQuery request, CancellationToken ct)
        {
            var cate = await _cateRepo.GetByIdAsync(request.Id, ct);
            if (cate == null) return Result<AlbumCateDto>.FailResult("相册分类不存在", 404);
            return Result<AlbumCateDto>.SuccessResult(MapCate(cate));
        }

        public async Task<Result<List<AlbumCateDto>>> Handle(GetAlbumCateListQuery request, CancellationToken ct)
        {
            var list = await _cateRepo.GetAllAsync(ct);
            return Result<List<AlbumCateDto>>.SuccessResult(list.Select(MapCate).ToList());
        }

        public async Task<Result<AlbumImageDto>> Handle(GetAlbumImageQuery request, CancellationToken ct)
        {
            var img = await _imageRepo.GetByIdAsync(request.Id, ct);
            if (img == null) return Result<AlbumImageDto>.FailResult("图片不存在", 404);
            return Result<AlbumImageDto>.SuccessResult(MapImage(img));
        }

        public async Task<Result<List<AlbumImageDto>>> Handle(GetAlbumImageListQuery request, CancellationToken ct)
        {
            var list = await _imageRepo.AsQueryable()
                .Where(i => i.CateId == request.CateId)
                .OrderByDescending(i => i.CreateTime).ToListAsync(ct);
            return Result<List<AlbumImageDto>>.SuccessResult(list.Select(MapImage).ToList());
        }

        private static AlbumCateDto MapCate(AlbumCate c) => new(c.Id, c.Name, c.Description, c.Cover, c.CreateTime);
        private static AlbumImageDto MapImage(AlbumImage i) => new(i.Id, i.CateId, i.Url,
            i.Thumbnail, i.Description, i.Size, i.Width, i.Height, i.CreateTime);
    }
}
