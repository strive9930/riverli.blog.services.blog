using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations;

/// <summary>
/// SiteNavigation 实体的 EF Core 映射配置
/// </summary>
public class SiteNavigationConfiguration : IEntityTypeConfiguration<SiteNavigation>
{
    public void Configure(EntityTypeBuilder<SiteNavigation> builder)
    {
        builder.ToTable("Blog_SiteNavigations");

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LinkUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Icon)
            .HasMaxLength(100);

        builder.Property(x => x.Target)
            .HasMaxLength(20)
            .HasDefaultValue("_self");

        builder.Property(x => x.SortOrder)
            .HasDefaultValue(0);

        builder.Property(x => x.IsVisible)
            .HasDefaultValue(true);

        // 按排序权重升序
        builder.HasIndex(x => x.SortOrder);

        // 软删除全局过滤器
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}