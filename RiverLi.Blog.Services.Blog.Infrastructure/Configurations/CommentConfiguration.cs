using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations;

/// <summary>
/// Comment 实体的 EF Core 映射配置
/// </summary>
public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Blog_Comments");

        builder.Property(x => x.ReviewerId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ReviewerName)
            .HasMaxLength(50);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(1000);

        // 把枚举存为字符串，便于阅读
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
    }
}
