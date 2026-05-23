using Microsoft.EntityFrameworkCore;
using MediatR;
using RiverLi.Blog.Infrastructure.Shared.Data;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Data
{
    public class BlogDbContext : RiverDbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ArticleCategory> ArticleCategories { get; set; }
        public DbSet<ArticleTag> ArticleTags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Wall> Walls { get; set; }
        public DbSet<WallCate> WallCates { get; set; }
        public DbSet<AlbumCate> AlbumCates { get; set; }
        public DbSet<AlbumImage> AlbumImages { get; set; }
        public DbSet<Link> Links { get; set; }
        public DbSet<LinkType> LinkTypes { get; set; }
        public DbSet<Swiper> Swipers { get; set; }
        public DbSet<Footprint> Footprints { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<Rss> Rsses { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<EnvConfig> EnvConfigs { get; set; }
        public DbSet<PageConfigEntity> PageConfigs { get; set; }
        public DbSet<Oss> Osses { get; set; }
        public DbSet<FileDetail> FileDetails { get; set; }

        public BlogDbContext(
            DbContextOptions<BlogDbContext> options,
            IMediator mediator,
            ICurrentUser currentUser)
            : base(options, mediator, currentUser)
        {
        }
    }
}
