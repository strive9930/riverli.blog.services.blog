using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class SwiperConfiguration : BaseEntityTypeConfiguration<Swiper>
    {
        public override void ConfigureEntity(EntityTypeBuilder<Swiper> builder)
        {
            builder.Property(x => x.Image).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.Url).HasMaxLength(500);
            builder.ToTable("swiper");
        }
    }
}
