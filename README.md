# RiverLi Blog — 博客服务

基于 .NET 9.0 + DDD + CQRS 的博客业务核心 API，提供文章、分类、标签、评论、媒体和站点导航的完整管理能力。

## 技术栈

| 层 | 技术 |
|---|---|
| 运行时 | .NET 9.0 |
| 框架 | ASP.NET Core Web API |
| 架构 | DDD（领域驱动设计）+ CQRS（MediatR） |
| 验证 | FluentValidation |
| 持久化 | EF Core 9 + Pomelo MySQL |
| 缓存 | IMemoryCache / Redis（可选） |
| 认证 | JWT Bearer |
| 文件存储 | 本地磁盘 / 阿里云 OSS（可切换） |
| 可观测性 | Serilog + OpenTelemetry |
| API 文档 | Scalar（OpenAPI） |
| 健康检查 | ASP.NET Core Health Checks |

## 项目结构

```
RiverLi.Blog.Services.Blog.sln
├── RiverLi.Blog.Services.Blog.Api/            ← Web API 层（Controller + 启动）
│   ├── Controllers/                           ← 6 个 API 控制器
│   ├── API.md                                 ← 完整 API 接口文档
│   ├── Program.cs
│   └── appsettings.json
├── RiverLi.Blog.Services.Blog.Application/    ← 应用层（CQRS Handler + DTO）
│   ├── Features/Articles/                     ← 文章命令/查询
│   ├── Features/Categories/                   ← 分类命令/查询
│   ├── Features/Comments/                     ← 评论命令/查询
│   ├── Features/Medias/                       ← 媒体命令/查询
│   ├── Features/SiteNavigations/              ← 站点导航命令/查询
│   ├── Features/Tags/                         ← 标签命令/查询
│   └── Common/Behaviors/                      ← ValidationBehavior、ConcurrencyBehavior
├── RiverLi.Blog.Services.Blog.Domain/         ← 领域层（聚合根、枚举、领域事件）
└── RiverLi.Blog.Services.Blog.Infrastructure/ ← 基础设施层（EF Core、存储、迁移）
```

## 功能清单

| 模块 | 功能 |
|---|---|
| **文章** | CRUD、草稿/发布状态切换、分页检索（关键词/分类/标签/排序）、详情（含标签） |
| **分类** | CRUD、树形结构、选项列表（公开匿名访问） |
| **标签** | CRUD、分页、下拉选项 |
| **评论** | 创建（JWT 身份）、审核（通过/驳回）、物理删除 |
| **媒体** | 图片上传（≤5MB）、通用文件上传（≤10MB）、分页媒体库、物理删除 |
| **站点导航** | 层级菜单 CRUD、显示/隐藏切换、公开/管理视图 |

> 所有接口均需 JWT 认证（标记 `[AllowAnonymous]` 的除外）。详细请求/响应格式见 [API.md](./RiverLi.Blog.Services.Blog.Api/API.md)。

## 快速开始

### 环境要求

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- MySQL 8.0+（或 MariaDB 10.6+）
- （可选）Redis 7.0+ 用于分布式缓存
- （可选）阿里云 OSS Bucket 用于云存储

### 1. 克隆并还原

```bash
git clone <repo-url>
cd riverli.blog.services.blog
dotnet restore
```

### 2. 配置

**切勿直接修改 `appsettings.json` 并提交敏感信息**。推荐使用 User Secrets 或环境变量覆盖占位符：

```bash
# JWT 密钥（至少 32 字符）
dotnet user-secrets set "Jwt:SecretKey" "YourSuperSecretKeyThatIs32CharsLong!"

# 数据库密码
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=3306;Database=riverli_blog;Uid=root;Pwd=YourRealPassword;AllowPublicKeyRetrieval=True;SslMode=Preferred;"

# 阿里云 OSS（可选，不使用则默认本地存储）
dotnet user-secrets set "AliyunOSS:AccessKeyId" "LTAI5t..."
dotnet user-secrets set "AliyunOSS:AccessKeySecret" "..."

# Redis（可选，不设置则仅使用内存缓存）
dotnet user-secrets set "Redis:ConnectionString" "localhost:6379"
```

或者创建 `appsettings.Development.json`（已在 `.gitignore` 中忽略）写入真实值。

### 3. 运行

```bash
dotnet run --project RiverLi.Blog.Services.Blog.Api
```

首次启动将自动执行数据库迁移并写入种子数据（默认分类、标签、示例文章）。

- API 直连：`http://localhost:5002`
- Scalar 文档：`http://localhost:5002/scalar/v1`（仅 Development 环境）

### 4. 切换云存储

默认使用 `LocalFileStorageService`（上传到 `wwwroot/uploads/`）。切换到阿里云 OSS：

编辑 `Program.cs`，注释/取消注释：

```csharp
//builder.Services.AddScoped<IStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IStorageService, AliyunOssStorageService>();
```

并确保 `AliyunOSS` 配置节点已正确填写。

## API 文档

完整接口文档（含请求/响应示例、枚举值、前端集成建议）请查看：

📄 **[API.md](./RiverLi.Blog.Services.Blog.Api/API.md)**

## 健康检查

| 端点 | 用途 |
|---|---|
| `GET /health` | 综合健康状态 |
| `GET /health/ready` | 就绪探针 |
| `GET /health/live` | 存活探针 |

## 许可证

[待定]
