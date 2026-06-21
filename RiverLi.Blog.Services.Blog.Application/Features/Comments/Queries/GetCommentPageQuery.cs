using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Comments.Queries;

/// <summary>
/// 评论分页列表
/// </summary>
public record GetCommentPageQuery : IRequest<PagedResult<CommentDto>>
{
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Status { get; init; }
    public Guid? ArticleId { get; init; }
}
