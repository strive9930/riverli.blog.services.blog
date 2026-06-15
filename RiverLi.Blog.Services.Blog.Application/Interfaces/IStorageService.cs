namespace RiverLi.Blog.Services.Blog.Application.Interfaces;

public interface IStorageService
{
    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="fileStream">文件流</param>
    /// <param name="extension">文件扩展名 (如 .jpg, .png)</param>
    /// <param name="contentType">MIME 类型 (如 image/jpeg)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>返回文件的可访问 URL</returns>
    Task<string> UploadAsync(Stream fileStream, string extension, string contentType, CancellationToken cancellationToken = default);
}