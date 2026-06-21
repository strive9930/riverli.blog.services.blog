using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using RiverLi.Blog.Services.Blog.Infrastructure.Data;

namespace RiverLi.Blog.Services.Blog.Api;

/// <summary>
/// EF Core 设计时工厂 — 用于 dotnet ef migrations 命令
/// 使用硬编码 ServerVersion 避免设计时连接数据库
/// </summary>
public class BlogDbContextFactory : IDesignTimeDbContextFactory<BlogDbContext>
{
    public BlogDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Port=3306;Database=riverli_blog;Uid=root;Pwd=River20260203;AllowPublicKeyRetrieval=True;SslMode=Preferred;";

        var optionsBuilder = new DbContextOptionsBuilder<BlogDbContext>();
        optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));

        return new BlogDbContext(optionsBuilder.Options, null!, null!);
    }
}
