using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Categories.Commands;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    private readonly IRepository<Category, Guid> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryHandler(IRepository<Category, Guid> repository, ICurrentUser currentUser, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Result<Guid>.FailResult("未登录用户无法创建分类");

        var category = new Category(request.Name, request.Slug, request.Description, request.ParentId, request.SortOrder);
        await _repository.AddAsync(category, cancellationToken);
        var saved = await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        if (!saved)
            return Result<Guid>.FailResult("分类保存失败，请重试");
        return Result<Guid>.SuccessResult(category.Id);
    }
}
