using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Queries;

/// <summary>
/// 评论分页列表 (后台审核用)
/// </summary>
public record GetCommentPageQuery(
    int PageIndex = 1,
    int PageSize = 10,
    string? Status = null,
    Guid? ArticleId = null
) : IRequest<Result<PagedResult<CommentDto>>>;
