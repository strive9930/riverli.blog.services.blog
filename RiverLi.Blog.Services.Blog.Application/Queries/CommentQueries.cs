using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using Microsoft.EntityFrameworkCore;

namespace RiverLi.Blog.Services.Blog.Application.Queries
{
    public record GetCommentQuery(Guid Id) : IRequest<Result<CommentDto>>;
    public record GetCommentListQuery(Guid? ArticleId = null, string? Status = null) : IRequest<Result<List<CommentDto>>>;
    public class GetCommentPagingQuery : PagedQuery, IRequest<PagedResult<CommentDto>>
    {
        public Guid? ArticleId { get; set; }
        public string? Status { get; set; }
    }

    public class CommentQueryHandler :
        IRequestHandler<GetCommentQuery, Result<CommentDto>>,
        IRequestHandler<GetCommentListQuery, Result<List<CommentDto>>>,
        IRequestHandler<GetCommentPagingQuery, PagedResult<CommentDto>>
    {
        private readonly IRepository<Comment, Guid> _repository;

        public CommentQueryHandler(IRepository<Comment, Guid> repository) => _repository = repository;

        public async Task<Result<CommentDto>> Handle(GetCommentQuery request, CancellationToken ct)
        {
            var comment = await _repository.GetByIdAsync(request.Id, ct);
            if (comment == null) return Result<CommentDto>.FailResult("评论不存在", 404);
            return Result<CommentDto>.SuccessResult(Map(comment));
        }

        public async Task<Result<List<CommentDto>>> Handle(GetCommentListQuery request, CancellationToken ct)
        {
            var query = _repository.AsQueryable();
            if (request.ArticleId.HasValue)
                query = query.Where(c => c.ArticleId == request.ArticleId.Value);
            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(c => c.Status == request.Status);
            query = query.OrderByDescending(c => c.CreateTime);
            var list = await query.ToListAsync(ct);
            return Result<List<CommentDto>>.SuccessResult(BuildTree(list));
        }

        public async Task<PagedResult<CommentDto>> Handle(GetCommentPagingQuery request, CancellationToken ct)
        {
            var query = _repository.AsQueryable();
            if (request.ArticleId.HasValue)
                query = query.Where(c => c.ArticleId == request.ArticleId.Value);
            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(c => c.Status == request.Status);
            query = query.OrderByDescending(c => c.CreateTime);

            var total = await query.CountAsync(ct);
            var items = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize).ToListAsync(ct);

            return PagedResult<CommentDto>.SuccessResult(
                items.Select(Map).ToList(), total,
                request.PageIndex, request.PageSize);
        }

        private List<CommentDto> BuildTree(List<Comment> comments)
        {
            var topLevel = comments.Where(c => c.ParentId == Guid.Empty).Select(Map).ToList();
            foreach (var dto in topLevel)
                dto.Replies = comments.Where(c => c.ParentId == dto.Id).Select(Map).ToList();
            return topLevel;
        }

        private static CommentDto Map(Comment c) => new(
            c.Id, c.ArticleId, c.Content, c.Email, c.Name, c.Url, c.Avatar,
            c.ParentId, c.Status, c.CreateTime);
    }
}
