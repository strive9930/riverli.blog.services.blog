using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class LinkTypeConfiguration : BaseEntityTypeConfiguration<LinkType>
    {
        public override void ConfigureEntity(EntityTypeBuilder<LinkType> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Icon).HasMaxLength(500);
            builder.ToTable("link_type");
        }
    }
}
