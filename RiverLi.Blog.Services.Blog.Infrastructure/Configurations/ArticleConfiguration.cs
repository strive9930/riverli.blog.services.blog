using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Infrastructure.Shared.Data.Configurations; // 假设您放在Shared里
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations
{
    public class ArticleConfiguration : BaseEntityTypeConfiguration<Article>
    {
        public override void ConfigureEntity(EntityTypeBuilder<Article> builder)
        {
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Summary).HasMaxLength(500);
            builder.Property(x => x.AuthorName).HasMaxLength(50);

            // 配置一对多关系
            builder.HasMany(x => x.Comments)
                   .WithOne()
                   .HasForeignKey(x => x.ArticleId)
                   .OnDelete(DeleteBehavior.Cascade);

            // 配置标签
            builder.HasMany(x => x.Tags)
                   .WithOne()
                   .HasForeignKey(x => x.ArticleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}