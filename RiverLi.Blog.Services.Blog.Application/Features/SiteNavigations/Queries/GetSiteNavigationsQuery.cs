using System.Collections.Generic;
using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Queries;

/// <summary>
/// 获取站点导航列表
/// </summary>
/// <param name="IsAdminView">是否为后台管理员视角（如果为 true，则包含隐藏的导航）</param>
public record GetSiteNavigationsQuery(bool IsAdminView = false) : IRequest<Result<List<SiteNavigationDto>>>;