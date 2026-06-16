using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Microsoft.Extensions.Configuration;
using RiverLi.Blog.Services.Blog.Application.Interfaces;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Storage;

/// <summary>
/// 阿里云 OSS 对象存储实现
/// </summary>
public class AliyunOssStorageService : IStorageService
{
    private readonly string _endpoint;
    private readonly string _accessKeyId;
    private readonly string _accessKeySecret;
    private readonly string _bucketName;
    private readonly string? _customDomain;

    public AliyunOssStorageService(IConfiguration configuration)
    {
        _endpoint = configuration["AliyunOSS:Endpoint"] ?? throw new ArgumentNullException("AliyunOSS:Endpoint 未配置");
        _accessKeyId = configuration["AliyunOSS:AccessKeyId"] ?? throw new ArgumentNullException("AliyunOSS:AccessKeyId 未配置");
        _accessKeySecret = configuration["AliyunOSS:AccessKeySecret"] ?? throw new ArgumentNullException("AliyunOSS:AccessKeySecret 未配置");
        _bucketName = configuration["AliyunOSS:BucketName"] ?? throw new ArgumentNullException("AliyunOSS:BucketName 未配置");
        _customDomain = configuration["AliyunOSS:CustomDomain"];
    }

    public async Task<string> UploadAsync(Stream fileStream, string extension, string contentType, CancellationToken cancellationToken = default)
    {
        // 1. 规整 OSS 中的存储路径 (按年月归档，规避同名冲突)
        var objectKey = $"uploads/{DateTime.UtcNow:yyyyMM}/{Guid.NewGuid().ToString("N")}{extension}";

        // 2. 初始化 OSS 客户端实例
        var client = new OssClient(_endpoint, _accessKeyId, _accessKeySecret);

        try
        {
            // 3. 设置元数据：🌟 极其重要！防止图片在浏览器中变成直接下载
            var metadata = new ObjectMetadata
            {
                ContentType = contentType
            };

            // 4. 利用 Task.Run 包裹 SDK 的同步上传方法，以适配异步管道并支持取消令牌
            await Task.Run(() => client.PutObject(_bucketName, objectKey, fileStream, metadata), cancellationToken);

            // 5. 组装并返回可供前端直接访问的最终 URL
            if (!string.IsNullOrEmpty(_customDomain))
            {
                // 如果配置了自定义域名/CDN 加速：https://media.riverli.blog/uploads/202606/xxx.png
                return $"{_customDomain.TrimEnd('/')}/{objectKey}";
            }

            // 使用阿里云默认的外网访问域名：https://riverli-blog.oss-cn-hangzhou.aliyuncs.com/uploads/202606/xxx.png
            var cleanEndpoint = _endpoint.Replace("http://", "").Replace("https://", "");
            return $"https://{_bucketName}.{cleanEndpoint}/{objectKey}";
        }
        catch (OssException ex)
        {
            throw new InvalidOperationException($"阿里云 OSS 存储服务异常: {ex.ErrorCode}, 消息: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("向阿里云 OSS 传输文件流时发生未知错误", ex);
        }
    }
}