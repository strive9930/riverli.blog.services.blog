using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Configurations;

public class FriendLinkConfiguration : IEntityTypeConfiguration<FriendLink>
{
    public void Configure(EntityTypeBuilder<FriendLink> builder)
    {
        builder.ToTable("Blog_FriendLinks");

        builder.Property(x => x.SiteName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SiteDescription).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Url).IsRequired().HasMaxLength(500);
        builder.Property(x => x.AvatarUrl).HasMaxLength(1000);
        builder.Property(x => x.Owner).IsRequired().HasMaxLength(50);
        builder.Property(x => x.RssUrl).HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.SortOrder);
    }
}
