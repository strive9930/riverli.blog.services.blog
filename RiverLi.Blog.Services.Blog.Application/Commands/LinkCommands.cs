using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Application.Commands
{
    public record CreateLinkCommand(string Name, string Url, string? Logo, string? Description,
        int TypeId, string? Email) : IRequest<Result<Guid>>;
    public record UpdateLinkCommand(Guid Id, string Name, string Url, string? Logo,
        string? Description, int TypeId, string? Email) : IRequest<Result>;
    public record DeleteLinkCommand(Guid Id) : IRequest<Result>;
    public record BatchDeleteLinkCommand(List<Guid> Ids) : IRequest<Result>;

    public record CreateLinkTypeCommand(string Name, string? Icon) : IRequest<Result<Guid>>;
    public record UpdateLinkTypeCommand(Guid Id, string Name, string? Icon) : IRequest<Result>;
    public record DeleteLinkTypeCommand(Guid Id) : IRequest<Result>;

    public class LinkCommandHandler :
        IRequestHandler<CreateLinkCommand, Result<Guid>>,
        IRequestHandler<UpdateLinkCommand, Result>,
        IRequestHandler<DeleteLinkCommand, Result>,
        IRequestHandler<BatchDeleteLinkCommand, Result>
    {
        private readonly IRepository<Link, Guid> _repository;
        public LinkCommandHandler(IRepository<Link, Guid> repository) => _repository = repository;

        public async Task<Result<Guid>> Handle(CreateLinkCommand request, CancellationToken ct)
        {
            var link = new Link(request.Name, request.Url, request.Logo,
                request.Description, request.TypeId, request.Email);
            await _repository.AddAsync(link, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result<Guid>.SuccessResult(link.Id);
        }

        public async Task<Result> Handle(UpdateLinkCommand request, CancellationToken ct)
        {
            var link = await _repository.GetByIdAsync(request.Id, ct);
            if (link == null) return Result.FailResult("友链不存在", 404);
            link.Update(request.Name, request.Url, request.Logo,
                request.Description, request.TypeId, request.Email);
            await _repository.UpdateAsync(link, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(DeleteLinkCommand request, CancellationToken ct)
        {
            await _repository.DeleteByIdAsync(request.Id, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(BatchDeleteLinkCommand request, CancellationToken ct)
        {
            foreach (var id in request.Ids)
                await _repository.DeleteByIdAsync(id, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }

    public class LinkTypeCommandHandler :
        IRequestHandler<CreateLinkTypeCommand, Result<Guid>>,
        IRequestHandler<UpdateLinkTypeCommand, Result>,
        IRequestHandler<DeleteLinkTypeCommand, Result>
    {
        private readonly IRepository<LinkType, Guid> _repository;
        public LinkTypeCommandHandler(IRepository<LinkType, Guid> repository) => _repository = repository;

        public async Task<Result<Guid>> Handle(CreateLinkTypeCommand request, CancellationToken ct)
        {
            var type = new LinkType(request.Name, request.Icon);
            await _repository.AddAsync(type, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result<Guid>.SuccessResult(type.Id);
        }

        public async Task<Result> Handle(UpdateLinkTypeCommand request, CancellationToken ct)
        {
            var type = await _repository.GetByIdAsync(request.Id, ct);
            if (type == null) return Result.FailResult("友链类型不存在", 404);
            type.Update(request.Name, request.Icon);
            await _repository.UpdateAsync(type, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(DeleteLinkTypeCommand request, CancellationToken ct)
        {
            await _repository.DeleteByIdAsync(request.Id, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }
}
