using MediatR;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using Microsoft.EntityFrameworkCore;

namespace RiverLi.Blog.Services.Blog.Application.Commands
{
    public record CreateCategoryCommand(string Name, string? Icon, Guid? ParentId = null) : IRequest<Result<Guid>>;
    public record UpdateCategoryCommand(Guid Id, string Name, string? Icon, Guid? ParentId) : IRequest<Result>;
    public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
    public record BatchDeleteCategoryCommand(List<Guid> Ids) : IRequest<Result>;

    public class CategoryCommandHandler :
        IRequestHandler<CreateCategoryCommand, Result<Guid>>,
        IRequestHandler<UpdateCategoryCommand, Result>,
        IRequestHandler<DeleteCategoryCommand, Result>,
        IRequestHandler<BatchDeleteCategoryCommand, Result>
    {
        private readonly IRepository<Category, Guid> _repository;

        public CategoryCommandHandler(IRepository<Category, Guid> repository) => _repository = repository;

        public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken ct)
        {
            var category = new Category(request.Name, request.Icon, request.ParentId);
            await _repository.AddAsync(category, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result<Guid>.SuccessResult(category.Id);
        }

        public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken ct)
        {
            var category = await _repository.GetByIdAsync(request.Id, ct);
            if (category == null) return Result.FailResult("分类不存在", 404);
            category.Update(request.Name, request.Icon, request.ParentId);
            await _repository.UpdateAsync(category, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken ct)
        {
            var category = await _repository.GetByIdAsync(request.Id, ct);
            if (category == null) return Result.FailResult("分类不存在", 404);
            await _repository.DeleteAsync(category, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(BatchDeleteCategoryCommand request, CancellationToken ct)
        {
            foreach (var id in request.Ids)
            {
                var category = await _repository.GetByIdAsync(id, ct);
                if (category != null) await _repository.DeleteAsync(category, ct);
            }
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }
}
