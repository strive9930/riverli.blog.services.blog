# 阶段1：构建环境 (必须使用包含完整 .NET SDK 的镜像)
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# 🌟 核心修改1：复制 Api 项目的 .csproj 文件，注意路径必须带上子文件夹前缀
COPY ["RiverLi.Blog.Services.Blog.Api/RiverLi.Blog.Services.Blog.Api.csproj", "RiverLi.Blog.Services.Blog.Api/"]

# 🌟 核心修改2：【非常重要】如果您的 Blog.Api 引用了 Application 或 Domain 等类库，
# 必须把它们的 .csproj 也提前复制进来，否则接下来的 restore 会直接报错。
# 如果没有独立类库，可以把下面这行删掉或注释掉：
# COPY ["RiverLi.Blog.Services.Blog.Application/RiverLi.Blog.Services.Blog.Application.csproj", "RiverLi.Blog.Services.Blog.Application/"]

# 🌟 核心修改3：执行依赖还原，精准指向容器内的 .csproj 路径
RUN dotnet restore "RiverLi.Blog.Services.Blog.Api/RiverLi.Blog.Services.Blog.Api.csproj"

# 复制整个解决方案下的所有源代码
COPY . .

# 🌟 核心修改4：把工作目录切换到 Api 文件夹内部，再执行发布命令
WORKDIR "/src/RiverLi.Blog.Services.Blog.Api"
RUN dotnet publish "RiverLi.Blog.Services.Blog.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 阶段2：极简运行环境 (仅包含运行时，体积不超过 100MB)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RiverLi.Blog.Services.Blog.Api.dll"]