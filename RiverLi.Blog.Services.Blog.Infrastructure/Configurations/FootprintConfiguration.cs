using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class FootprintConfiguration : BaseEntityTypeConfiguration<Footprint>
    {
        public override void ConfigureEntity(EntityTypeBuilder<Footprint> builder)
        {
            builder.Property(x => x.Location).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Content).HasMaxLength(2000);
            builder.Property(x => x.Images).HasColumnType("text");
            builder.Property(x => x.Longitude).HasColumnType("decimal(10,7)");
            builder.Property(x => x.Latitude).HasColumnType("decimal(10,7)");
            builder.ToTable("footprint");
        }
    }
}
