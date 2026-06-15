using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RiverLi.Blog.Services.Blog.Application.Interfaces;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Storage;

public class LocalFileStorageService : IStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalFileStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> UploadAsync(Stream fileStream, string extension, string contentType, CancellationToken cancellationToken = default)
    {
        // 1. 生成基于时间的相对安全的防重名文件名
        var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid().ToString("N")[..8]}{extension}";
        
        // 2. 确定物理保存路径
        var uploadPath = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var filePath = Path.Combine(uploadPath, fileName);

        // 3. 写入磁盘
        using (var fileStreamOnDisk = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fileStreamOnDisk, cancellationToken);
        }

        // 4. 拼接返回对外可访问的 URL
        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = $"{request?.Scheme}://{request?.Host}{request?.PathBase}";
        
        return $"{baseUrl}/uploads/{fileName}";
    }
}