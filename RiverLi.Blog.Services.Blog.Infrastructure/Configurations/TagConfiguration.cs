using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations;

/// <summary>
/// Tag 实体的 EF Core 映射配置
/// </summary>
public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Blog_Tags");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Slug)
            .IsRequired()
            .HasMaxLength(100);

        // 软删除全局过滤器
        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
