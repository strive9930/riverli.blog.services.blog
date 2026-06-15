using System;

namespace RiverLi.Blog.Services.Blog.Application.Common.Dto;

/// <summary>
/// 媒体文件 DTO
/// </summary>
public record MediaDto(
    Guid Id,
    string FileName,
    string Url,
    string ContentType,
    long FileSize,
    string UploadedBy,
    DateTime CreateTime
);
