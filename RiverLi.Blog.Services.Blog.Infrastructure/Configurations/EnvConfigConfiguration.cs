using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class EnvConfigConfiguration : BaseEntityTypeConfiguration<EnvConfig>
    {
        public override void ConfigureEntity(EntityTypeBuilder<EnvConfig> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Value).HasColumnType("text");
            builder.ToTable("env_config");
        }
    }
}
