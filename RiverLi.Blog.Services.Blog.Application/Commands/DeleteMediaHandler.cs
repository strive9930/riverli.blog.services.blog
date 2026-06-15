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

public class DeleteMediaHandler : IRequestHandler<DeleteMediaCommand, Result>
{
    private readonly IRepository<Media, Guid> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<DeleteMediaHandler> _logger;

    public DeleteMediaHandler(
        IRepository<Media, Guid> repository,
        ICurrentUser currentUser,
        IWebHostEnvironment env,
        ILogger<DeleteMediaHandler> logger)
    {
        _repository = repository;
        _currentUser = currentUser;
        _env = env;
        _logger = logger;
    }

    public async Task<Result> Handle(DeleteMediaCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Result.FailResult("未登录用户无法删除文件");

        var media = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (media == null)
            return Result.FailResult("文件不存在");

        // 删除物理文件
        var fullPath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), media.StoragePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);

        await _repository.DeleteAsync(media, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation("文件已删除: {FileName} -> {Path}", media.FileName, media.StoragePath);
        return Result.SuccessResult();
    }
}
