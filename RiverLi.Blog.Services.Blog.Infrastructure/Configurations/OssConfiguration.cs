using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class OssConfiguration : BaseEntityTypeConfiguration<Oss>
    {
        public override void ConfigureEntity(EntityTypeBuilder<Oss> builder)
        {
            builder.Property(x => x.Platform).HasMaxLength(50).IsRequired();
            builder.Property(x => x.AccessKey).HasMaxLength(200);
            builder.Property(x => x.SecretKey).HasMaxLength(200);
            builder.Property(x => x.Bucket).HasMaxLength(200);
            builder.Property(x => x.Domain).HasMaxLength(500);
            builder.Property(x => x.BasePath).HasMaxLength(500);
            builder.ToTable("oss");
        }
    }
}
