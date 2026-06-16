using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Medias.Queries;

/// <summary>
/// 媒体文件分页查询
/// </summary>
public record GetMediaPageQuery(
    int PageIndex = 1,
    int PageSize = 12,
    string? Keyword = null,
    string? ContentType = null
) : IRequest<Result<PagedResult<MediaDto>>>;
