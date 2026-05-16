using Microsoft.EntityFrameworkCore;
using MediatR;
//using RiverLi.DDD.Core.Domain.Interfaces;
using RiverLi.Blog.Infrastructure.Shared.Data; // 引用通用 RiverDbContext
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Data
{
    public class BlogDbContext : RiverDbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public BlogDbContext(
            DbContextOptions<BlogDbContext> options,
            IMediator mediator,
            ICurrentUser currentUser)
            : base(options, mediator, currentUser)
        {
        }

        // 提示：RiverDbContext 会自动扫描当前程序集的 Configuration，无需手动 Apply
    }
}