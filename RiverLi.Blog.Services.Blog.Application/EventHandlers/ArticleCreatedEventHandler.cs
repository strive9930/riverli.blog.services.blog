using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using RiverLi.Blog.Services.Blog.Domain.Events;

namespace RiverLi.Blog.Services.Blog.Application.EventHandlers;

/// <summary>
/// 文章创建事件处理器 (订阅者)
/// 负责处理文章落库成功后的副作用操作，如：清空 Redis 缓存、推送到搜索引擎索引、发送通知等
/// </summary>
public class ArticleCreatedEventHandler : INotificationHandler<ArticleCreatedEvent>
{
    private readonly ILogger<ArticleCreatedEventHandler> _logger;
    private readonly IDistributedCache? _cache;

    /// <summary>文章列表缓存 Key 前缀 (与 Redis InstanceName 组合后为: RiverBlog_Article_List)</summary>
    private const string ArticleListCachePrefix = "Article_List";

    public ArticleCreatedEventHandler(
        ILogger<ArticleCreatedEventHandler> logger,
        IDistributedCache? cache = null)
    {
        _logger = logger;
        _cache = cache;
    }

    /// <summary>
    /// 处理领域事件
    /// </summary>
    public async Task Handle(ArticleCreatedEvent notification, CancellationToken cancellationToken)
    {
        // 1. 打印日志，验证事件是否成功被进程内的 MediatR 捕获
        _logger.LogInformation(
            "【领域事件触发成功】检测到新文章发布！事件ID: {EventId}, 发生时间(UTC): {EventTime}, 文章ID: {ArticleId}, 标题: {Title}, 作者ID: {AuthorId}",
            notification.EventId,
            notification.EventTime,
            notification.ArticleId,
            notification.Title,
            notification.AuthorId);

        // 2. 清理 Redis 缓存：新文章发布后，文章列表缓存已失效
        await ClearArticleListCacheAsync(cancellationToken);

        // 3. 这里是处理其他“副作用”的绝佳位置。
        // 例如：
        // await _searchEngineService.SyncArticleAsync(notification.ArticleId, cancellationToken);

        await Task.CompletedTask;
    }

    /// <summary>
    /// 清除文章列表相关缓存 (缓存失效策略)
    /// </summary>
    private async Task ClearArticleListCacheAsync(CancellationToken cancellationToken)
    {
        if (_cache == null)
        {
            _logger.LogDebug("未配置 Redis 缓存，跳过缓存清除");
            return;
        }

        try
        {
            // 删除首页文章列表缓存 (覆盖最常见的访问场景)
            await _cache.RemoveAsync($"{ArticleListCachePrefix}_Page_1", cancellationToken);
            _logger.LogInformation("已清除文章列表缓存: {CacheKey}", $"{ArticleListCachePrefix}_Page_1");
        }
        catch (Exception ex)
        {
            // 缓存清除失败不应阻塞主流程
            _logger.LogWarning(ex, "清除文章列表缓存时发生异常，缓存可能未及时更新");
        }
    }
}