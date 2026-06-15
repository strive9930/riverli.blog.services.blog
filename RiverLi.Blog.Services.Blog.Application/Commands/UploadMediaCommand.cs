using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

/// <summary>
/// 上传媒体文件命令
/// </summary>
public record UploadMediaCommand(
    string FileName,
    string ContentType,
    byte[] FileData
) : IRequest<Result<Guid>>;
