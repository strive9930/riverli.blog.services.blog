using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class RssConfiguration : BaseEntityTypeConfiguration<Rss>
    {
        public override void ConfigureEntity(EntityTypeBuilder<Rss> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Url).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.Logo).HasMaxLength(500);
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.ToTable("rss");
        }
    }
}
