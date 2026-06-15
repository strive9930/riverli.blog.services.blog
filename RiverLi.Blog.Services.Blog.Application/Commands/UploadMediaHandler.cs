using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public class UploadMediaHandler : IRequestHandler<UploadMediaCommand, Result<Guid>>
{
    private readonly IRepository<Media, Guid> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UploadMediaHandler> _logger;

    // 允许上传的文件类型
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".mp4", ".webm", ".pdf" };

    public UploadMediaHandler(
        IRepository<Media, Guid> repository,
        ICurrentUser currentUser,
        IWebHostEnvironment env,
        ILogger<UploadMediaHandler> logger)
    {
        _repository = repository;
        _currentUser = currentUser;
        _env = env;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.Id == null)
            return Result<Guid>.FailResult("未登录用户无法上传文件");

        var ext = Path.GetExtension(request.FileName).ToLowerInvariant();
        if (Array.IndexOf(AllowedExtensions, ext) < 0)
            return Result<Guid>.FailResult($"不支持的文件类型: {ext}");

        if (request.FileData == null || request.FileData.Length == 0)
            return Result<Guid>.FailResult("文件数据为空");

        // 按日期分目录存储
        var dateDir = DateTime.UtcNow.ToString("yyyy/MM");
        var storageDir = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", dateDir);
        Directory.CreateDirectory(storageDir);

        var uniqueName = $"{Guid.NewGuid():N}{ext}";
        var relativePath = Path.Combine("uploads", dateDir, uniqueName);
        var fullPath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), relativePath);

        await File.WriteAllBytesAsync(fullPath, request.FileData, cancellationToken);

        var media = new Media(
            request.FileName,
            relativePath.Replace('\\', '/'),
            $"/{relativePath.Replace('\\', '/')}",
            request.ContentType,
            request.FileData.Length,
            _currentUser.Id.ToString()!
        );

        await _repository.AddAsync(media, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation("文件上传成功: {FileName} -> {Path}", request.FileName, relativePath);
        return Result<Guid>.SuccessResult(media.Id);
    }
}
