using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Medias.Commands;

public class UploadMediaHandler : IRequestHandler<UploadMediaCommand, Result<Guid>>
{
    private readonly IRepository<Media, Guid> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UploadMediaHandler> _logger;

    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg", ".mp4", ".webm", ".pdf" };

    public UploadMediaHandler(
        IRepository<Media, Guid> repository,
        ICurrentUser currentUser,
        IUnitOfWork unitOfWork,
        IWebHostEnvironment env,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UploadMediaHandler> logger)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _env = env;
        _httpContextAccessor = httpContextAccessor;
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

        var dateDir = DateTime.UtcNow.ToString("yyyy/MM");
        var storageDir = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", dateDir);
        Directory.CreateDirectory(storageDir);

        var uniqueName = $"{Guid.NewGuid():N}{ext}";
        var relativePath = Path.Combine("uploads", dateDir, uniqueName);
        var fullPath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), relativePath);

        await File.WriteAllBytesAsync(fullPath, request.FileData, cancellationToken);

        var httpContext = _httpContextAccessor.HttpContext;
        var baseUrl = httpContext != null
            ? $"{httpContext.Request.Scheme}://{httpContext.Request.Host}"
            : "http://localhost:5002";
        var url = $"{baseUrl}/{relativePath.Replace('\\', '/')}";

        var media = new Media(
            request.FileName,
            relativePath.Replace('\\', '/'),
            url,
            request.ContentType,
            request.FileData.Length,
            _currentUser.Id.ToString()!
        );

        await _repository.AddAsync(media, cancellationToken);
        var saved = await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        if (!saved)
        {
            if (File.Exists(fullPath)) File.Delete(fullPath);
            return Result<Guid>.FailResult("文件保存失败，请重试");
        }

        _logger.LogInformation("文件上传成功: {FileName} -> {Path}", request.FileName, relativePath);
        return Result<Guid>.SuccessResult(media.Id);
    }
}
