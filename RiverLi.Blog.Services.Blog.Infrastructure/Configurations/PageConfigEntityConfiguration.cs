using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class PageConfigEntityConfiguration : BaseEntityTypeConfiguration<PageConfigEntity>
    {
        public override void ConfigureEntity(EntityTypeBuilder<PageConfigEntity> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Config).HasColumnType("text");
            builder.ToTable("page_config");
        }
    }
}
