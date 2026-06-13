using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
// 如果您的 BaseEntityTypeConfiguration 在外部共享库中，请确保引入对应的命名空间
// using RiverLi.Blog.Infrastructure.Shared.Data.Configurations; 

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations;

/// <summary>
/// Article 实体的 EF Core 映射配置
/// </summary>
public class ArticleConfiguration : IEntityTypeConfiguration<Article> 
// 如果您有基类，请改为继承基类，例如：: BaseEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        // 1. 表名配置 (可选，EF 默认会用 DbSet 的名字，显式指定更严谨)
        builder.ToTable("Blog_Articles");

        // 2. 字段限制配置 (防止数据库生成 max 类型的列，优化性能)
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Summary)
            .HasMaxLength(500);

        builder.Property(x => x.AuthorId)
            .HasMaxLength(50);

        builder.Property(x => x.AuthorName)
            .HasMaxLength(50);

        builder.Property(x => x.CoverUrl)
            .HasMaxLength(1000); // URL 可能比较长

        // 枚举存为字符串，便于阅读
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // 软删除全局过滤器
        builder.HasQueryFilter(x => !x.IsDeleted);

        // 3. 🌟 核心：导航属性的后备字段 (Backing Fields) 配置
        // 告诉 EF Core，当从数据库查出数据时，请把值塞进这些 private 的集合中
        builder.Metadata.FindNavigation(nameof(Article.Comments))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
            
        builder.Metadata.FindNavigation(nameof(Article.Tags))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // 4. 🌟 核心：一对多关系与级联删除
        // 配置 Article 与 Comment 的一对多关系，文章删除时，下面的评论一并级联删除
        builder.HasMany(x => x.Comments)
            .WithOne() // Comment 实体中如果没有 Article 的导航属性，这里留空即可
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        // 配置 Article 与 ArticleTag (中间表) 的一对多关系
        builder.HasMany(x => x.Tags)
            .WithOne()
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}