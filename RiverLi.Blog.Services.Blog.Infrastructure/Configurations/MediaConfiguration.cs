using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations;

/// <summary>
/// Media 实体的 EF Core 映射配置
/// </summary>
public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.ToTable("Blog_Media");

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.StoragePath)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.UploadedBy)
            .HasMaxLength(50);
    }
}
