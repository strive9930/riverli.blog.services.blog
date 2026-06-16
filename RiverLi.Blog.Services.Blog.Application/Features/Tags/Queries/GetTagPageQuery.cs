using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Tags.Queries;

/// <summary>
/// 标签分页列表
/// </summary>
public record GetTagPageQuery(
    int PageIndex = 1,
    int PageSize = 20,
    string? Keyword = null
) : IRequest<Result<PagedResult<TagDto>>>;
