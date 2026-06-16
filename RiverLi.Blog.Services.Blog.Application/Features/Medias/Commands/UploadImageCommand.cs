using System.IO;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Medias.Commands;

/// <summary>
/// 上传图片命令 (用于文章封面或正文插�?
/// </summary>
/// <param name="Stream">图片文件�?/param>
/// <param name="Extension">文件扩展�?(�?.)</param>
/// <param name="ContentType">MIME 类型</param>
public record UploadImageCommand(
    Stream Stream,
    string Extension,
    string ContentType
) : IRequest<Result<string>>;
