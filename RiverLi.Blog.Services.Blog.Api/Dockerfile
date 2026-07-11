# 阶段1：构建环境 (因为是独立仓库，直接复制根目录即可)
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# 复制项目文件并还原
COPY ["RiverLi.Blog.Services.Blog.Api.csproj", "./"]
RUN dotnet restore "RiverLi.Blog.Services.Blog.Api.csproj"

# 复制其余代码并发布
COPY . .
RUN dotnet publish "RiverLi.Blog.Services.Blog.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 阶段2：极简运行环境
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RiverLi.Blog.Services.Blog.Api.dll"]