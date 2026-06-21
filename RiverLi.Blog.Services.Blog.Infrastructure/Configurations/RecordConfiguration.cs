using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations;

public class RecordConfiguration : IEntityTypeConfiguration<Record>
{
    public void Configure(EntityTypeBuilder<Record> builder)
    {
        builder.ToTable("Blog_Records");

        builder.Property(x => x.Content).IsRequired().HasMaxLength(2000);
        builder.Property(x => x.ImageUrls).HasMaxLength(2000);
        builder.Property(x => x.Location).HasMaxLength(100);

        builder.HasIndex(x => x.CreateTime);
    }
}
