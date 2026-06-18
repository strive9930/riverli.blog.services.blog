using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Queries;

public class GetSiteNavigationsHandler : IRequestHandler<GetSiteNavigationsQuery, Result<List<SiteNavigationDto>>>
{
    private readonly IRepository<SiteNavigation, Guid> _repository;

    public GetSiteNavigationsHandler(IRepository<SiteNavigation, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<SiteNavigationDto>>> Handle(GetSiteNavigationsQuery request, CancellationToken cancellationToken)
    {
        var navigations = await _repository.GetAllAsync(cancellationToken); 

        if (!request.IsAdminView)
        {
            navigations = navigations.Where(x => x.IsVisible).ToList();
        }

        // 1. 先转成平铺的 DTO 列表
        var allItems = navigations
            .OrderBy(x => x.SortOrder)
            .Select(x => new SiteNavigationDto {
                Id = x.Id,
                ParentId = x.ParentId,
                Title = x.Title,
                LinkUrl = x.LinkUrl,
                Icon = x.Icon,
                SortOrder = x.SortOrder,
                Target = x.Target,
                IsVisible = x.IsVisible
            }).ToList();

        // 2. 🌟 内存组装树形结构
        var tree = BuildTree(allItems, null);

        return Result<List<SiteNavigationDto>>.SuccessResult(tree);
    }

    // 递归构建树的私有方法
    private List<SiteNavigationDto> BuildTree(List<SiteNavigationDto> allItems, Guid? parentId)
    {
        return allItems
            .Where(x => x.ParentId == parentId)
            .Select(x => {
                x.Children = BuildTree(allItems, x.Id);
                return x;
            })
            .ToList();
    }
}