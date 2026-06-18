using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public class DeleteSiteNavigationHandler : IRequestHandler<DeleteSiteNavigationCommand, Result<bool>>
{
    private readonly IRepository<SiteNavigation, Guid> _repository;

    public DeleteSiteNavigationHandler(IRepository<SiteNavigation, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> Handle(DeleteSiteNavigationCommand request, CancellationToken cancellationToken)
    {
        // 1. 获取要删除的实体
        var navigation = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (navigation == null)
        {
            return Result<bool>.FailResult("要删除的导航数据不存在");
        }

        // 2. 执行删除
        await _repository.DeleteAsync(navigation, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Result<bool>.SuccessResult(true);
    }
}