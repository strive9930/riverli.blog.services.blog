using System.Text;
using System.Text.RegularExpressions;

namespace RiverLi.Blog.Services.Blog.Application.Common;

/// <summary>
/// URL Slug 生成工具
/// </summary>
public static class SlugHelper
{
    /// <summary>
    /// 将标题转换为 URL 友好的 slug
    /// 处理中英文混合、特殊字符，确保唯一性
    /// </summary>
    public static string Generate(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        // 1. 转小写
        var slug = title.ToLowerInvariant();

        // 2. 替换中文标点、特殊符号为连字符
        slug = Regex.Replace(slug, @"[\p{P}\p{S}]+", "-");

        // 3. 将空白字符替换为连字符
        slug = Regex.Replace(slug, @"\s+", "-");

        // 4. 移除连续连字符
        slug = Regex.Replace(slug, @"-{2,}", "-");

        // 5. 移除首尾连字符
        slug = slug.Trim('-');

        // 6. 如果处理后为空，使用 "untitled"
        if (string.IsNullOrEmpty(slug))
            slug = "untitled";

        // 7. 限制长度
        if (slug.Length > 200)
            slug = slug[..200];

        // 8. 追加短哈希防止中文标题全被过滤后 slug 相同
        var hash = Math.Abs(title.GetHashCode()).ToString("x")[..6];
        slug = $"{slug}-{hash}";

        return slug;
    }
}
