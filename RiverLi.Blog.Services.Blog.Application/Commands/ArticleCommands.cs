using MediatR;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Application.Commands
{
    public record UpdateArticleCommand(Guid Id, string Title, string Content, string? Description,
        string? Cover, string Config, List<int>? CateIds, List<string>? Tags) : IRequest<Result>;

    public record DeleteArticleCommand(Guid Id) : IRequest<Result>;
    public record BatchDeleteArticleCommand(List<Guid> Ids) : IRequest<Result>;
    public record IncrementViewCountCommand(Guid Id) : IRequest<Result>;

    public class ArticleCommandHandler :
        IRequestHandler<UpdateArticleCommand, Result>,
        IRequestHandler<DeleteArticleCommand, Result>,
        IRequestHandler<BatchDeleteArticleCommand, Result>,
        IRequestHandler<IncrementViewCountCommand, Result>
    {
        private readonly IRepository<Article, Guid> _repository;

        public ArticleCommandHandler(IRepository<Article, Guid> repository) => _repository = repository;

        public async Task<Result> Handle(UpdateArticleCommand request, CancellationToken ct)
        {
            var article = await _repository.GetByIdAsync(request.Id, ct);
            if (article == null) return Result.FailResult("文章不存在", 404);
            article.Update(request.Title, request.Content, request.Description, request.Cover, request.Config);
            if (request.CateIds != null) article.SetCategories(request.CateIds);
            if (request.Tags != null) article.SetTags(request.Tags);
            await _repository.UpdateAsync(article, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(DeleteArticleCommand request, CancellationToken ct)
        {
            var article = await _repository.GetByIdAsync(request.Id, ct);
            if (article == null) return Result.FailResult("文章不存在", 404);
            await _repository.DeleteAsync(article, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(BatchDeleteArticleCommand request, CancellationToken ct)
        {
            foreach (var id in request.Ids)
            {
                var article = await _repository.GetByIdAsync(id, ct);
                if (article != null) await _repository.DeleteAsync(article, ct);
            }
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(IncrementViewCountCommand request, CancellationToken ct)
        {
            var article = await _repository.GetByIdAsync(request.Id, ct);
            if (article == null) return Result.FailResult("文章不存在", 404);
            article.IncrementViewCount();
            await _repository.UpdateAsync(article, ct);
            await _repository.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }
}
