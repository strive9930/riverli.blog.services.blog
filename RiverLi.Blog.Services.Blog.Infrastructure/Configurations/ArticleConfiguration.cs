using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations;

/// <summary>
/// Article 实体的 EF Core 映射配置
/// </summary>
public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Blog_Articles");

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasIndex(x => x.Slug).IsUnique();

        builder.Property(x => x.Summary)
            .HasMaxLength(500);

        builder.Property(x => x.AuthorId)
            .HasMaxLength(50);

        builder.Property(x => x.AuthorName)
            .HasMaxLength(50);

        builder.Property(x => x.CoverUrl)
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.ScheduledPublishTime)
            .HasColumnType("datetime(6)");

        // 软删除全局过滤器
        builder.HasQueryFilter(x => !x.IsDeleted);

        // 导航属性：Category (只读，不可通过 Article 修改 Category)
        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // 导航属性的后备字段 (Backing Fields)
        builder.Metadata.FindNavigation(nameof(Article.Comments))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);
            
        builder.Metadata.FindNavigation(nameof(Article.Tags))?
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // 一对多关系与级联删除
        builder.HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Tags)
            .WithOne()
            .HasForeignKey(x => x.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
