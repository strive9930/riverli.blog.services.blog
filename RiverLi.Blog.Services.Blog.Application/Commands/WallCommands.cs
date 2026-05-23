using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;

namespace RiverLi.Blog.Services.Blog.Application.Commands
{
    // Wall commands
    public record CreateWallCommand(string Content, string Email, string Name, string? Url,
        string? Avatar, int CateId, string? Ip) : IRequest<Result<Guid>>;
    public record DeleteWallCommand(Guid Id) : IRequest<Result>;
    public record BatchDeleteWallCommand(List<Guid> Ids) : IRequest<Result>;

    // WallCate commands
    public record CreateWallCateCommand(string Name, string? Icon, string? Color) : IRequest<Result<Guid>>;
    public record UpdateWallCateCommand(Guid Id, string Name, string? Icon, string? Color) : IRequest<Result>;
    public record DeleteWallCateCommand(Guid Id) : IRequest<Result>;

    public class WallCommandHandler :
        IRequestHandler<CreateWallCommand, Result<Guid>>,
        IRequestHandler<DeleteWallCommand, Result>,
        IRequestHandler<BatchDeleteWallCommand, Result>
    {
        private readonly IRepository<Wall, Guid> _repository;
        public WallCommandHandler(IRepository<Wall, Guid> repository) => _repository = repository;

        public async Task<Result<Guid>> Handle(CreateWallCommand request, CancellationToken ct)
        {
            var wall = new Wall(request.Content, request.Email, request.Name,
                request.Url, request.Avatar, request.CateId, request.Ip);
            await _repository.AddAsync(wall, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result<Guid>.SuccessResult(wall.Id);
        }

        public async Task<Result> Handle(DeleteWallCommand request, CancellationToken ct)
        {
            await _repository.DeleteByIdAsync(request.Id, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(BatchDeleteWallCommand request, CancellationToken ct)
        {
            foreach (var id in request.Ids)
                await _repository.DeleteByIdAsync(id, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }

    public class WallCateCommandHandler :
        IRequestHandler<CreateWallCateCommand, Result<Guid>>,
        IRequestHandler<UpdateWallCateCommand, Result>,
        IRequestHandler<DeleteWallCateCommand, Result>
    {
        private readonly IRepository<WallCate, Guid> _repository;
        public WallCateCommandHandler(IRepository<WallCate, Guid> repository) => _repository = repository;

        public async Task<Result<Guid>> Handle(CreateWallCateCommand request, CancellationToken ct)
        {
            var cate = new WallCate(request.Name, request.Icon, request.Color);
            await _repository.AddAsync(cate, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result<Guid>.SuccessResult(cate.Id);
        }

        public async Task<Result> Handle(UpdateWallCateCommand request, CancellationToken ct)
        {
            var cate = await _repository.GetByIdAsync(request.Id, ct);
            if (cate == null) return Result.FailResult("留言分类不存在", 404);
            cate.Update(request.Name, request.Icon, request.Color);
            await _repository.UpdateAsync(cate, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(DeleteWallCateCommand request, CancellationToken ct)
        {
            await _repository.DeleteByIdAsync(request.Id, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }
}
