using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class ArticleConfiguration : BaseEntityTypeConfiguration<Article>
    {
        public override void ConfigureEntity(EntityTypeBuilder<Article> builder)
        {
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.Cover).HasMaxLength(500);
            builder.Property(x => x.AuthorName).HasMaxLength(50);
            builder.Property(x => x.Config).HasColumnType("text");

            builder.HasMany(x => x.Comments)
                   .WithOne()
                   .HasForeignKey(x => x.ArticleId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Tags)
                   .WithOne()
                   .HasForeignKey(x => x.ArticleId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Categories)
                   .WithOne()
                   .HasForeignKey(x => x.ArticleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
