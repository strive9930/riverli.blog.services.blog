using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class LinkConfiguration : BaseEntityTypeConfiguration<Link>
    {
        public override void ConfigureEntity(EntityTypeBuilder<Link> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Url).HasMaxLength(500).IsRequired();
            builder.Property(x => x.Logo).HasMaxLength(500);
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("pending");
            builder.Property(x => x.Email).HasMaxLength(200);
            builder.ToTable("link");
        }
    }
}
