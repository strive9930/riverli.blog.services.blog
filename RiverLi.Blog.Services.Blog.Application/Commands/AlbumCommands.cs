using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Application.Commands
{
    public record CreateAlbumCateCommand(string Name, string? Description, string? Cover) : IRequest<Result<Guid>>;
    public record UpdateAlbumCateCommand(Guid Id, string Name, string? Description, string? Cover) : IRequest<Result>;
    public record DeleteAlbumCateCommand(Guid Id) : IRequest<Result>;

    public record CreateAlbumImageCommand(int CateId, string Url, string? Thumbnail,
        string? Description, int Size, int Width, int Height) : IRequest<Result<Guid>>;
    public record DeleteAlbumImageCommand(Guid Id) : IRequest<Result>;

    public class AlbumCateCommandHandler :
        IRequestHandler<CreateAlbumCateCommand, Result<Guid>>,
        IRequestHandler<UpdateAlbumCateCommand, Result>,
        IRequestHandler<DeleteAlbumCateCommand, Result>
    {
        private readonly IRepository<AlbumCate, Guid> _repository;
        public AlbumCateCommandHandler(IRepository<AlbumCate, Guid> repository) => _repository = repository;

        public async Task<Result<Guid>> Handle(CreateAlbumCateCommand request, CancellationToken ct)
        {
            var cate = new AlbumCate(request.Name, request.Description, request.Cover);
            await _repository.AddAsync(cate, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result<Guid>.SuccessResult(cate.Id);
        }

        public async Task<Result> Handle(UpdateAlbumCateCommand request, CancellationToken ct)
        {
            var cate = await _repository.GetByIdAsync(request.Id, ct);
            if (cate == null) return Result.FailResult("相册分类不存在", 404);
            cate.Update(request.Name, request.Description, request.Cover);
            await _repository.UpdateAsync(cate, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(DeleteAlbumCateCommand request, CancellationToken ct)
        {
            await _repository.DeleteByIdAsync(request.Id, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }

    public class AlbumImageCommandHandler :
        IRequestHandler<CreateAlbumImageCommand, Result<Guid>>,
        IRequestHandler<DeleteAlbumImageCommand, Result>
    {
        private readonly IRepository<AlbumImage, Guid> _repository;
        public AlbumImageCommandHandler(IRepository<AlbumImage, Guid> repository) => _repository = repository;

        public async Task<Result<Guid>> Handle(CreateAlbumImageCommand request, CancellationToken ct)
        {
            var img = new AlbumImage(request.CateId, request.Url, request.Thumbnail,
                request.Description, request.Size, request.Width, request.Height);
            await _repository.AddAsync(img, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result<Guid>.SuccessResult(img.Id);
        }

        public async Task<Result> Handle(DeleteAlbumImageCommand request, CancellationToken ct)
        {
            await _repository.DeleteByIdAsync(request.Id, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }
}
