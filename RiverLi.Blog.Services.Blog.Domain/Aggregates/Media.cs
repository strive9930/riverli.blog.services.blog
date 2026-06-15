using System;
using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates;

/// <summary>
/// 媒体文件聚合根
/// </summary>
public class Media : BaseEntity<Guid>, IAggregateRoot
{
    /// <summary>原始文件名</summary>
    public string FileName { get; private set; }

    /// <summary>服务端存储路径 (相对路径)</summary>
    public string StoragePath { get; private set; }

    /// <summary>公开访问 URL</summary>
    public string Url { get; private set; }

    /// <summary>MIME 类型</summary>
    public string ContentType { get; private set; }

    /// <summary>文件大小 (字节)</summary>
    public long FileSize { get; private set; }

    /// <summary>上传者 ID</summary>
    public string UploadedBy { get; private set; }

    private Media() { } // EF Core

    public Media(string fileName, string storagePath, string url, string contentType, long fileSize, string uploadedBy)
    {
        FileName = fileName;
        StoragePath = storagePath;
        Url = url;
        ContentType = contentType;
        FileSize = fileSize;
        UploadedBy = uploadedBy;
    }
}
