using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Infrastructure.Shared.Data;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Infrastructure.Shared.Extensions;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Infrastructure.Data;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace RiverLi.Blog.Services.Blog.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // 1. OpenAPI (Scalar) 配置
            // 注意：这里只是描述文档，使用 Microsoft.OpenApi.Models 命名空间
            builder.Services.AddOpenApi(options =>
            {
                
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    // 显式定义多个 Server 入口
                    document.Servers = new List<OpenApiServer>
                    {
                        // 这样在 5000 端口查看文档时，默认请求地址就是网关自己
                        new() { Url = "http://localhost:5002", Description = "本地直接访问 (用于内部开发)" },
                        new() { Url = "http://localhost:5000/api/v1/blog", Description = "通过网关访问 (推荐调试使用)" }
                    };
                    
                    document.Components ??= new OpenApiComponents();
                    
                    // 定义安全方案（仅用于 UI 显示，不会触发认证逻辑冲突）
                    var schemeName = "Bearer"; 
                    document.Components.SecuritySchemes.Add(schemeName, new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "请输入 JWT Token"
                    });

                    document.SecurityRequirements.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecurityScheme 
                        { 
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = schemeName } 
                        }] = Array.Empty<string>()
                    });

                    return Task.CompletedTask;
                });
            });

            // 2. 基础服务注册
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("GatewayPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:5000") // 允许网关来的请求
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            
            // 3. 数据库配置
            builder.Services.AddDbContext<BlogDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });
            builder.Services.AddScoped<RiverDbContext>(provider => provider.GetRequiredService<BlogDbContext>());
            
            // 4. 业务逻辑服务注册
            builder.Services.AddScoped(typeof(IRepository<,>), typeof(RiverLi.Blog.Infrastructure.Shared.Repositories.EfRepository<,>));
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateArticleHandler).Assembly));

            // 5. 身份验证与授权 (核心：确保只调用一次)
            // ✅ 如果 AddRiverJwtAuthentication 内部报错 Scheme exists，请确保项目中没有其他地方调用 .AddJwtBearer("Bearer")
            builder.Services.AddRiverJwtAuthentication(builder.Configuration);
            builder.Services.AddAuthorization(); // 只保留一个

            // 6. 网关转发头配置
            builder.Services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            // 7. 其他共享基础设施
            builder.Services.AddRiverTracing("RiverLi.BlogService");
            
            if (!string.IsNullOrEmpty(builder.Configuration["Redis:ConnectionString"]))
            {
                builder.Services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = builder.Configuration["Redis:ConnectionString"];
                    options.InstanceName = "RiverBlog_";
                });
            }

            var app = builder.Build();

            // 8. 中间件管道配置 (顺序至关重要)
            
            // 必须放在最前面，处理网关转发信息
            app.UseForwardedHeaders();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi(); // 生成 /openapi/v1.json
                app.MapScalarApiReference(options =>
                {
                    options
                        .WithTitle("RiverLi Blog API")
                        .WithTheme(ScalarTheme.Moon)
                        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });
            }
            
            app.UseCors("GatewayPolicy");
            //app.UseHttpsRedirection();

            // 认证在授权之前
            app.UseAuthentication(); 
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}