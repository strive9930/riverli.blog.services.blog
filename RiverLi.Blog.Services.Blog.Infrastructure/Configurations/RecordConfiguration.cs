using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class RecordConfiguration : BaseEntityTypeConfiguration<Record>
    {
        public override void ConfigureEntity(EntityTypeBuilder<Record> builder)
        {
            builder.Property(x => x.Content).HasMaxLength(2000).IsRequired();
            builder.Property(x => x.Images).HasColumnType("text");
            builder.ToTable("record");
        }
    }
}
