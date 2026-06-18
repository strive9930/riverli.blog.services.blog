using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public class CreateSiteNavigationHandler : IRequestHandler<CreateSiteNavigationCommand, Result<Guid>>
{
    private readonly IRepository<SiteNavigation, Guid> _repository;

    public CreateSiteNavigationHandler(IRepository<SiteNavigation, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> Handle(CreateSiteNavigationCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.LinkUrl))
        {
            return Result<Guid>.FailResult("导航标题和链接不能为空");
        }

        var navigation = new SiteNavigation(
            request.ParentId,
            request.Title,
            request.LinkUrl,
            request.Icon,
            request.SortOrder,
            request.Target,
            request.IsVisible
        );

        await _repository.AddAsync(navigation, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Result<Guid>.SuccessResult(navigation.Id);
    }
}