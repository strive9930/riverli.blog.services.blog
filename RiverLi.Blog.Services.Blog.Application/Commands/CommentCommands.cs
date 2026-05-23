using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Application.Commands
{
    public record CreateCommentCommand(Guid? ArticleId, string Content, string Email, string Name,
        string? Url, string? Avatar, Guid ParentId, string? Ip, string? UserAgent) : IRequest<Result<Guid>>;

    public record ReplyCommentCommand(Guid? ArticleId, string Content, string Email, string Name,
        string? Url, string? Avatar, Guid ParentId, string? Ip, string? UserAgent) : IRequest<Result<Guid>>;

    public record DeleteCommentCommand(Guid Id) : IRequest<Result>;
    public record BatchDeleteCommentCommand(List<Guid> Ids) : IRequest<Result>;
    public record AuditCommentCommand(Guid Id, string Status) : IRequest<Result>;

    public class CommentCommandHandler :
        IRequestHandler<CreateCommentCommand, Result<Guid>>,
        IRequestHandler<ReplyCommentCommand, Result<Guid>>,
        IRequestHandler<DeleteCommentCommand, Result>,
        IRequestHandler<BatchDeleteCommentCommand, Result>,
        IRequestHandler<AuditCommentCommand, Result>
    {
        private readonly IRepository<Comment, Guid> _repository;

        public CommentCommandHandler(IRepository<Comment, Guid> repository) => _repository = repository;

        public async Task<Result<Guid>> Handle(CreateCommentCommand request, CancellationToken ct)
        {
            var comment = new Comment(request.ArticleId, request.Content, request.Email,
                request.Name, request.Url, request.Avatar, Guid.Empty, request.Ip, request.UserAgent);
            await _repository.AddAsync(comment, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result<Guid>.SuccessResult(comment.Id);
        }

        public async Task<Result<Guid>> Handle(ReplyCommentCommand request, CancellationToken ct)
        {
            var comment = new Comment(request.ArticleId, request.Content, request.Email,
                request.Name, request.Url, request.Avatar, request.ParentId, request.Ip, request.UserAgent);
            await _repository.AddAsync(comment, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result<Guid>.SuccessResult(comment.Id);
        }

        public async Task<Result> Handle(DeleteCommentCommand request, CancellationToken ct)
        {
            await _repository.DeleteByIdAsync(request.Id, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(BatchDeleteCommentCommand request, CancellationToken ct)
        {
            foreach (var id in request.Ids)
                await _repository.DeleteByIdAsync(id, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(AuditCommentCommand request, CancellationToken ct)
        {
            var comment = await _repository.GetByIdAsync(request.Id, ct);
            if (comment == null) return Result.FailResult("评论不存在", 404);
            if (request.Status == "approved") comment.Approve();
            else comment.Reject();
            await _repository.UpdateAsync(comment, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }
}
