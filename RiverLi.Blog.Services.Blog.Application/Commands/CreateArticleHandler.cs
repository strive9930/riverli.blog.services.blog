using MediatR;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
//using RiverLi.DDD.Core.Domain.Interfaces;
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
            // 1. 获取当前用户
            if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
            {
                return Result<Guid>.FailResult("未登录用户无法发布文章");
            }

            var authorId = _currentUser.Id; // 假设 ICurrentUser.Id 是 Guid?
            var authorName = _currentUser.UserName ?? "Unknown";

            // 2. 构建领域对象
            var article = new Article(
                request.Title,
                request.Content,
                request.Summary,
                request.CoverUrl,
                authorId,
                authorName
            );

            // 3. 添加标签 (领域行为)
            // 这里需要在 Article 中添加 AddTag 方法，保持封装性
            // foreach(var tag in request.Tags) article.AddTag(tag); 

            // 4. 持久化
            await _repository.AddAsync(article, cancellationToken);

            // 5. 提交事务 (自动触发审计字段填充、领域事件)
            await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Result<Guid>.SuccessResult(article.Id);
        }
    }
}