using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class ConfigConfiguration : BaseEntityTypeConfiguration<Config>
    {
        public override void ConfigureEntity(EntityTypeBuilder<Config> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Value).HasColumnType("text");
            builder.Property(x => x.Notes).HasMaxLength(500);
            builder.ToTable("config");
        }
    }
}
