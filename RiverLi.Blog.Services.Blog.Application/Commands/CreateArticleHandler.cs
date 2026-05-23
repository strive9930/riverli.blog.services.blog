using MediatR;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Application.Commands
{
    public class CreateArticleHandler : IRequestHandler<CreateArticleCommand, Result<Guid>>
    {
        private readonly IRepository<Article, Guid> _repository;
        private readonly ICurrentUser _currentUser;

        public CreateArticleHandler(IRepository<Article, Guid> repository, ICurrentUser currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<Result<Guid>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
                return Result<Guid>.FailResult("未登录用户无法发布文章");

            var authorId = _currentUser.Id;
            var authorName = _currentUser.UserName ?? "Unknown";

            var article = new Article(
                request.Title,
                request.Content,
                request.Description,
                request.Cover,
                request.Config,
                authorId,
                authorName
            );

            foreach (var tag in request.Tags)
                article.AddTag(tag);

            await _repository.AddAsync(article, cancellationToken);
            await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Result<Guid>.SuccessResult(article.Id);
        }
    }
}
