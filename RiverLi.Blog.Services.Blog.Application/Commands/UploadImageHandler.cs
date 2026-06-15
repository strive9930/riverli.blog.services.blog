using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public class UploadImageHandler : IRequestHandler<UploadImageCommand, Result<string>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UploadImageHandler> _logger;

    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public UploadImageHandler(
        ICurrentUser currentUser,
        IWebHostEnvironment env,
        ILogger<UploadImageHandler> logger)
    {
        _currentUser = currentUser;
        _env = env;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
            return Result<string>.FailResult("未登录用户无法上传图片");

        var ext = request.Extension.ToLowerInvariant();
        if (Array.IndexOf(AllowedImageExtensions, ext) < 0)
            return Result<string>.FailResult($"不支持的图片格式: {ext}，仅支持 {string.Join(", ", AllowedImageExtensions)}");

        // 按日期分目录
        var dateDir = DateTime.UtcNow.ToString("yyyy/MM");
        var storageDir = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "images", dateDir);
        Directory.CreateDirectory(storageDir);

        var uniqueName = $"{Guid.NewGuid():N}{ext}";
        var relativePath = Path.Combine("uploads", "images", dateDir, uniqueName).Replace('\\', '/');
        var fullPath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), relativePath);

        await using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await request.Stream.CopyToAsync(fileStream, cancellationToken);
        }

        var url = $"/{relativePath}";
        _logger.LogInformation("图片上传成功: {Extension} -> {Path} ({Size} bytes)", ext, relativePath, request.Stream.Length);

        return Result<string>.SuccessResult(url);
    }
}
