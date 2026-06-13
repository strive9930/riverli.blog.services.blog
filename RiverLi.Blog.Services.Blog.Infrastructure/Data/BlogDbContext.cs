using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MediatR;
using RiverLi.Blog.Infrastructure.Shared.Data;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Data;

/// <summary>
/// Blog 微服务专属数据库上下文
/// </summary>
public class BlogDbContext : RiverDbContext
{
    private readonly IMediator _mediator;

    
    public DbSet<Article> Articles => Set<Article>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ArticleTag> ArticleTags => Set<ArticleTag>();

    public BlogDbContext(
        DbContextOptions<BlogDbContext> options,
        IMediator mediator,
        ICurrentUser currentUser) // 从基类继承下来的当前用户信息接口
        : base(options, mediator, currentUser)
    {
        _mediator = mediator;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 自动扫描当前程序集（Infrastructure 层）下所有实现了 IEntityTypeConfiguration 的类
        // 也就是我们之前写的 ArticleConfiguration 会在这里自动生效
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BlogDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. 自动处理审计字段和软删除
        ProcessAuditableAndSoftDeleteEntities();

        // 2. 🌟 在数据落库之前，拦截并分发所有的领域事件
        await DispatchDomainEventsAsync(cancellationToken);

        // 3. 执行真正的数据库写盘 (调用基类的 SaveChangesAsync)
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 处理审计字段 (UpdateTime) 与软删除 (IsDeleted, DeleteTime)
    /// </summary>
    private void ProcessAuditableAndSoftDeleteEntities()
    {
        var entries = ChangeTracker.Entries().Where(e =>
            e.State == EntityState.Added || 
            e.State == EntityState.Modified || 
            e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            // 自动更新修改时间
            if (entry.State == EntityState.Modified && entry.Entity is IAuditableEntity auditableEntity)
            {
                // 利用 dynamic 动态调用您 BaseEntity 里的 UpdateModifyTime
                dynamic entity = entry.Entity;
                entity.UpdateModifyTime();
            }

            // EF Core 原生删除转为软删除
            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete softDeleteEntity)
            {
                entry.State = EntityState.Modified; // 拦截：将状态从删除强制改回修改
                dynamic entity = entry.Entity;
                entity.MarkAsDeleted(); // 调用您底层的标记删除方法
            }
        }
    }

    /// <summary>
    /// 提取并分发领域事件
    /// </summary>
    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        // A. 找出 ChangeTracker 中所有带有 DomainEvents 的实体
        var domainEntities = ChangeTracker.Entries()
            .Select(x => x.Entity)
            .Where(x =>
            {
                var prop = x.GetType().GetProperty("DomainEvents", BindingFlags.Public | BindingFlags.Instance);
                if (prop == null) return false;

                var events = prop.GetValue(x) as IEnumerable<object>;
                return events != null && events.Any();
            })
            .ToList();

        if (domainEntities.Count == 0) return;

        // B. 提取所有事件并强制转换为 MediatR 的 INotification
        var domainEvents = domainEntities
            .SelectMany(x =>
            {
                var prop = x.GetType().GetProperty("DomainEvents");
                return (IEnumerable<object>)prop!.GetValue(x)!;
            })
            .Cast<INotification>()
            .ToList();

        // C. 🌟 极其重要：在发布事件前，必须先清空实体的事件集合！
        // 防止 EventHandler 内部再次触发 SaveChanges 导致 StackOverflow 无限递归。
        foreach (var entity in domainEntities)
        {
            var method = entity.GetType().GetMethod("ClearDomainEvents");
            method?.Invoke(entity, null);
        }

        // D. 遍历事件列表，通过 MediatR 进行进程内广播
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}