using Microsoft.EntityFrameworkCore;
using FluentValidation;
using RiverLi.Blog.Infrastructure.Shared.Data;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Infrastructure.Shared.Extensions;
using RiverLi.Blog.Infrastructure.Shared.OpenApi;
using RiverLi.Blog.Services.Blog.Application.Common.Behaviors;
using RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;
using RiverLi.Blog.Services.Blog.Application.Interfaces;
using RiverLi.Blog.Services.Blog.Infrastructure.Data;
using RiverLi.Blog.Services.Blog.Infrastructure.Storage;

namespace RiverLi.Blog.Services.Blog.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ==========================================
        // 1. 共享基建注入
        // ==========================================
        
        builder.Services.AddLoggingSupport(builder.Configuration, "BlogService");
        builder.Services.AddRiverTracing("RiverLi.BlogService");
        
        builder.Services.AddInfrastructureSharedServices(options => 
        {
            options.EnableGlobalExceptionHandler = true;
            options.EnableCors = true;
            options.AllowedOrigins = new[] { "http://localhost:5000", "http://localhost:3000", "http://localhost:5173" };
            
            options.EnableOpenApiDocumentation = true;
            options.ScalarTitle = "RiverLi Blog 业务核心 API";
            options.ScalarVersion = "v1";
            options.ScalarDescription = "博客文章、分类、标签与评论接口";
        });

        builder.Services.AddHealthCheckSupport(builder.Configuration);
        builder.Services.AddApiSelfReporting();

        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers(); 
        
        builder.Services.AddEndpointsApiExplorer();
        // 🌟 补上这行：注册内存缓存服务，提供 IMemoryCache 的具体实现
        builder.Services.AddMemoryCache();
        // ==========================================
        // 2. 业务专属注入
        // ==========================================

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("未获取到连接字符串");
        builder.Services.AddDbContext<BlogDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        // 注册服务
        builder.Services.AddHttpContextAccessor();

        #region 文件存储

        builder.Services.AddScoped<IStorageService, LocalFileStorageService>(); 
        //  🚀 一键替换为阿里云 OSS 存储服务
        //builder.Services.AddScoped<IStorageService, AliyunOssStorageService>();

        #endregion
        
        builder.Services.AddScoped<RiverDbContext>(provider => provider.GetRequiredService<BlogDbContext>());
        
        builder.Services.AddScoped(typeof(IRepository<,>), typeof(RiverLi.Blog.Infrastructure.Shared.Repositories.EfRepository<,>));
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateArticleHandler).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(ConcurrencyBehavior<,>));
        });
        
        // FluentValidation 自动扫描注册
        builder.Services.AddValidatorsFromAssemblyContaining<CreateArticleCommandValidator>();

        builder.Services.AddRiverJwtAuthentication(builder.Configuration);
        builder.Services.AddAuthorization(); 

        if (!string.IsNullOrEmpty(builder.Configuration["Redis:ConnectionString"]))
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration["Redis:ConnectionString"];
                options.InstanceName = "RiverBlog_";
            });
        }

        // ==========================================
        // 3. 中间件管道
        // ==========================================
        var app = builder.Build();
        
        // 在 app.Build() 之后，开启静态文件访问
        app.UseStaticFiles();
        
        // 启动时自动迁移数据库并初始化种子数据
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<BlogDbContext>();
                var logger = services.GetRequiredService<ILogger<Program>>();

                if (dbContext.Database.GetPendingMigrations().Any())
                {
                    logger.LogInformation("检测到未应用的迁移，开始执行...");
                    await dbContext.Database.MigrateAsync();
                    await DbSeeder.SeedAsync(dbContext, logger);
                }
                else
                {
                    await DbSeeder.SeedAsync(dbContext, logger);
                }
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "在执行数据库自动迁移或种子数据初始化时发生错误！");
            }
        }
        
        app.UseInfrastructureSharedMiddlewares();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.MapInfrastructureSharedEndpoints(options => {
            options.EnableOpenApiDocumentation = app.Environment.IsDevelopment();
            options.ScalarTitle = "RiverLi Blog 业务核心 API";
        });

        await app.RunAsync();
    }
}