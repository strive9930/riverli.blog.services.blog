using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Categories.Commands;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IRepository<Category, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public DeleteCategoryHandler(IRepository<Category, Guid> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Result.FailResult("未登录用户无法删除分类");

        var category = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            return Result.FailResult("分类不存在");

        category.MarkAsDeleted();
        var saved = await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        if (!saved)
            return Result.FailResult("删除失败，请重试");
        return Result.SuccessResult();
    }
}
