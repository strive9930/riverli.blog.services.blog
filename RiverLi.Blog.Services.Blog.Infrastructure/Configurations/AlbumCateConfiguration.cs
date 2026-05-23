using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class AlbumCateConfiguration : BaseEntityTypeConfiguration<AlbumCate>
    {
        public override void ConfigureEntity(EntityTypeBuilder<AlbumCate> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.Cover).HasMaxLength(500);
            builder.ToTable("album_cate");
        }
    }
}
