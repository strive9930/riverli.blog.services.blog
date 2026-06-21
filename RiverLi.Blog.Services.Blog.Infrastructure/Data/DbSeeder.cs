using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RiverLi.Blog.Services.Blog.Application.Common;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Domain.Enum;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Data;

/// <summary>
/// 数据库初始化种子数据
/// 在程序首次启动 / 数据库迁移完成后执行，确保基础数据就位
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(BlogDbContext dbContext, ILogger logger, CancellationToken ct = default)
    {
        logger.LogInformation("开始检查并初始化种子数据...");

        await SeedCategoriesAsync(dbContext, logger, ct);
        await SeedTagsAsync(dbContext, logger, ct);
        await SeedSampleArticleAsync(dbContext, logger, ct);

        logger.LogInformation("种子数据初始化完成。");
    }

    /// <summary>
    /// 确保"未分类"等默认分类存在
    /// </summary>
    private static async Task SeedCategoriesAsync(BlogDbContext dbContext, ILogger logger, CancellationToken ct)
    {
        if (await dbContext.Categories.IgnoreQueryFilters().AnyAsync(c => c.Name == "未分类", ct))
        {
            logger.LogInformation("默认分类已存在，跳过。");
            return;
        }

        var categories = new[]
        {
            new Category("未分类",     "uncategorized", "默认分类，未归类文章归属此处",       null,          0),
            new Category("技术博客",   "tech",          "技术文章、编程心得与架构设计",       null,          1),
            new Category("随笔杂谈",   "essay",         "生活感悟、读书笔记与个人思考",       null,          2),
            new Category("开源项目",   "opensource",    "开源项目介绍、贡献指南与版本发布",   null,          3),
        };

        await dbContext.Categories.AddRangeAsync(categories, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("已初始化 {Count} 个默认分类。", categories.Length);
    }

    /// <summary>
    /// 预设常用技术标签
    /// </summary>
    private static async Task SeedTagsAsync(BlogDbContext dbContext, ILogger logger, CancellationToken ct)
    {
        if (await dbContext.Tags.IgnoreQueryFilters().AnyAsync(ct))
        {
            logger.LogInformation("标签数据已存在，跳过。");
            return;
        }

        var tags = new[]
        {
            new Tag("C#",          "csharp"),
            new Tag("ASP.NET Core","aspnet-core"),
            new Tag("DDD",         "ddd"),
            new Tag("微服务",      "microservices"),
            new Tag("Entity Framework Core", "efcore"),
            new Tag("MediatR",     "mediatr"),
            new Tag("Clean Architecture", "clean-architecture"),
            new Tag("Docker",      "docker"),
            new Tag("Redis",       "redis"),
            new Tag("MySQL",       "mysql"),
        };

        await dbContext.Tags.AddRangeAsync(tags, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("已初始化 {Count} 个默认标签。", tags.Length);
    }

    /// <summary>
    /// 创建一篇示例文章，方便开发调试
    /// </summary>
    private static async Task SeedSampleArticleAsync(BlogDbContext dbContext, ILogger logger, CancellationToken ct)
    {
        if (await dbContext.Articles.IgnoreQueryFilters().AnyAsync(ct))
        {
            logger.LogInformation("文章数据已存在，跳过示例文章。");
            return;
        }

        // 找到"未分类"作为默认分类
        var uncategorized = await dbContext.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Slug == "uncategorized", ct);

        if (uncategorized == null)
        {
            logger.LogWarning("未找到'未分类'分类，跳过示例文章。");
            return;
        }

        // 找几个标签挂到示例文章上
        var tagIds = await dbContext.Tags
            .IgnoreQueryFilters()
            .Where(t => new[] { "csharp", "ddd", "clean-architecture" }.Contains(t.Slug))
            .Select(t => t.Id)
            .ToListAsync(ct);

        var article = new Article(
            title:       "欢迎使用 RiverLi Blog 博客系统",
            slug:        SlugHelper.Generate("欢迎使用 RiverLi Blog 博客系统"),
            content:     """
                         ## 欢迎 👋

                         这是一篇自动生成的示例文章。

                         ### 技术栈

                         本博客系统基于以下技术构建：

                         - **.NET 9** — 高性能运行时
                         - **Clean Architecture** — 清晰的四层架构
                         - **DDD (领域驱动设计)** — 充血模型 + 聚合根
                         - **CQRS** — MediatR 命令查询职责分离
                         - **Entity Framework Core** — ORM 数据访问
                         - **MySQL** — 关系型数据库
                         - **Redis** — 分布式缓存
                         - **JWT** — 无状态身份认证

                         ### 项目结构

                         ```
                         Api → Application → Domain
                          ↓
                         Infrastructure → Shared
                         ```

                         祝编码愉快！🚀
                         """,
            summary:     "RiverLi Blog 博客系统的欢迎文章，介绍项目技术栈与架构",
            coverUrl:    null,
            categoryId:  uncategorized.Id,
            authorId:    "system",
            authorName:  "系统管理员"
        );

        // 发布示例文章
        article.ChangeStatus(ArticleStatus.Published);

        // 绑定标签
        if (tagIds.Any())
        {
            article.SetTags(tagIds);
        }

        await dbContext.Articles.AddAsync(article, ct);
        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("已创建一篇示例文章: '{Title}'。", article.Title);
    }
}
