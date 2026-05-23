using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class WallCateConfiguration : BaseEntityTypeConfiguration<WallCate>
    {
        public override void ConfigureEntity(EntityTypeBuilder<WallCate> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Icon).HasMaxLength(500);
            builder.Property(x => x.Color).HasMaxLength(20);
            builder.ToTable("wall_cate");
        }
    }
}
