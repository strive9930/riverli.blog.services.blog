using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public class CreateArticleHandler : IRequestHandler<CreateArticleCommand, Result<Guid>>
{
    private readonly IRepository<Article, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public CreateArticleHandler(
        IRepository<Article, Guid> repository, 
        ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        // 1. 获取并校验当前用户状态
        if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
        {
            return Result<Guid>.FailResult("未登录用户无法发布文章");
        }

        // 🌟 适配：将当前登录用户的 ID 安全地转换为 string，以匹配 Article 实体要求
        var authorId = _currentUser.Id.ToString()!; 
        var authorName = _currentUser.UserName ?? "匿名创作者";

        // 2. 严格校验业务逻辑 (防御式编程)
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result<Guid>.FailResult("文章正文内容不能为空");
        }

        // 3. 构建领域对象 (充血模型，会自动触发 ArticleCreatedEvent 领域事件)
        var article = new Article(
            title: request.Title,
            content: request.Content,
            summary: request.Summary,
            coverUrl: request.CoverUrl,
            categoryId: request.CategoryId, // 🌟 挂载分类
            authorId: authorId,
            authorName: authorName
        );

        // 4. 添加标签映射 (🌟 调用我们在聚合根里写好的高内聚领域行为)
        if (request.TagIds != null && request.TagIds.Any())
        {
            article.SetTags(request.TagIds);
        }

        // 5. 将新聚合根加入仓储追踪上下文
        await _repository.AddAsync(article, cancellationToken);

        // 6. 提交事务 (扣动扳机)
        // 这一步将极其丝滑地触发以下连锁反应：
        // 自动计算 CreateTime 等审计字段 -> 提取领域事件 -> 进程内广播给 ArticleCreatedEventHandler -> 真正落盘并提交事务
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        // 7. 成功返回新文章的主键
        return Result<Guid>.SuccessResult(article.Id);
    }
}