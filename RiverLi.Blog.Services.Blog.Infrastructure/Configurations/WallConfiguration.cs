using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class WallConfiguration : BaseEntityTypeConfiguration<Wall>
    {
        public override void ConfigureEntity(EntityTypeBuilder<Wall> builder)
        {
            builder.Property(x => x.Content).IsRequired().HasMaxLength(2000);
            builder.Property(x => x.Email).HasMaxLength(200);
            builder.Property(x => x.Name).HasMaxLength(100);
            builder.Property(x => x.Url).HasMaxLength(500);
            builder.Property(x => x.Avatar).HasMaxLength(500);
            builder.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("pending");
            builder.Property(x => x.Ip).HasMaxLength(50);
            builder.ToTable("wall");
        }
    }
}
