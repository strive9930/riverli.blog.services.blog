using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public class UpdateSiteNavigationHandler : IRequestHandler<UpdateSiteNavigationCommand, Result<bool>>
{
    private readonly IRepository<SiteNavigation, Guid> _repository;

    public UpdateSiteNavigationHandler(IRepository<SiteNavigation, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<Result<bool>> Handle(UpdateSiteNavigationCommand request, CancellationToken cancellationToken)
    {
        // 1. 业务校验：标题和链接不可为空
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.LinkUrl))
        {
            return Result<bool>.FailResult("导航标题和链接不能为空");
        }

        // 2. 获取领域实体
        // 💡 提示：请根据您的 IRepository 实际方法名微调，如 GetByIdAsync 等
        var navigation = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (navigation == null)
        {
            return Result<bool>.FailResult("要修改的导航数据不存在");
        }

        // 3. 执行领域行为 (调用我们之前在 SiteNavigation 中写好的 Update 方法)
        navigation.Update(
            request.ParentId,
            request.Title,
            request.LinkUrl,
            request.Icon,
            request.SortOrder,
            request.Target,
            request.IsVisible
        );

        // 4. 持久化
        await _repository.UpdateAsync(navigation, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Result<bool>.SuccessResult(true);
    }
}