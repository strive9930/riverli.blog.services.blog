using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly IRepository<Category, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public UpdateCategoryHandler(IRepository<Category, Guid> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Result.FailResult("未登录用户无法修改分类");

        var category = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            return Result.FailResult("分类不存在");

        category.Update(request.Name, request.Slug, request.Description, request.ParentId, request.SortOrder);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Result.SuccessResult();
    }
}
