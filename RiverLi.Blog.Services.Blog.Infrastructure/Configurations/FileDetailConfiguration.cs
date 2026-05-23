using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class FileDetailConfiguration : BaseEntityTypeConfiguration<FileDetail>
    {
        public override void ConfigureEntity(EntityTypeBuilder<FileDetail> builder)
        {
            builder.Property(x => x.FileName).HasMaxLength(500).IsRequired();
            builder.Property(x => x.FilePath).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.FileUrl).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.FileType).HasMaxLength(100);
            builder.Property(x => x.Platform).HasMaxLength(50);
            builder.ToTable("file_detail");
        }
    }
}
