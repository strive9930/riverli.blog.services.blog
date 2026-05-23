using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class AlbumImageConfiguration : BaseEntityTypeConfiguration<AlbumImage>
    {
        public override void ConfigureEntity(EntityTypeBuilder<AlbumImage> builder)
        {
            builder.Property(x => x.Url).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.Thumbnail).HasMaxLength(1000);
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.ToTable("album_image");
        }
    }
}
