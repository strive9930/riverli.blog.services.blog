using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.FriendLinks.Queries;

/// <summary>
/// 友链分页查询（公开：仅返回已审核）
/// </summary>
public record GetFriendLinkPageQuery(
    int PageIndex = 1,
    int PageSize = 20,
    bool AdminView = false
) : IRequest<PagedResult<FriendLinkDto>>;
