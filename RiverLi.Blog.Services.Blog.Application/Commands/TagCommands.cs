using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Application.Commands
{
    public record CreateTagCommand(string Name, string? Color) : IRequest<Result<Guid>>;
    public record UpdateTagCommand(Guid Id, string Name, string? Color) : IRequest<Result>;
    public record DeleteTagCommand(Guid Id) : IRequest<Result>;
    public record BatchDeleteTagCommand(List<Guid> Ids) : IRequest<Result>;

    public class TagCommandHandler :
        IRequestHandler<CreateTagCommand, Result<Guid>>,
        IRequestHandler<UpdateTagCommand, Result>,
        IRequestHandler<DeleteTagCommand, Result>,
        IRequestHandler<BatchDeleteTagCommand, Result>
    {
        private readonly IRepository<Tag, Guid> _repository;

        public TagCommandHandler(IRepository<Tag, Guid> repository) => _repository = repository;

        public async Task<Result<Guid>> Handle(CreateTagCommand request, CancellationToken ct)
        {
            var tag = new Tag(request.Name, request.Color);
            await _repository.AddAsync(tag, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result<Guid>.SuccessResult(tag.Id);
        }

        public async Task<Result> Handle(UpdateTagCommand request, CancellationToken ct)
        {
            var tag = await _repository.GetByIdAsync(request.Id, ct);
            if (tag == null) return Result.FailResult("标签不存在", 404);
            tag.Update(request.Name, request.Color);
            await _repository.UpdateAsync(tag, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(DeleteTagCommand request, CancellationToken ct)
        {
            var tag = await _repository.GetByIdAsync(request.Id, ct);
            if (tag == null) return Result.FailResult("标签不存在", 404);
            await _repository.DeleteAsync(tag, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(BatchDeleteTagCommand request, CancellationToken ct)
        {
            foreach (var id in request.Ids)
            {
                var tag = await _repository.GetByIdAsync(id, ct);
                if (tag != null) await _repository.DeleteAsync(tag, ct);
            }
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }
}
