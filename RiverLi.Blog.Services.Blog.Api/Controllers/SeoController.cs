using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Api.Controllers;

[ApiController]
[Route("api/blog/[controller]")]
public class SeoController : ControllerBase
{
    private readonly IRepository<Article, Guid> _articleRepo;

    public SeoController(IRepository<Article, Guid> articleRepo) => _articleRepo = articleRepo;

    /// <summary>RSS Feed — 最近 20 篇已发布文章</summary>
    [HttpGet("rss")]
    [ResponseCache(Duration = 3600)]
    public async Task<IActionResult> GetRss(CancellationToken ct)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var articles = await _articleRepo.AsQueryable()
            .Where(a => !a.IsDeleted && a.Status == ArticleStatus.Published)
            .OrderByDescending(a => a.CreateTime)
            .Take(20)
            .Select(a => new { a.Id, a.Title, a.Slug, a.Summary, a.CreateTime, a.UpdateTime })
            .ToListAsync(ct);

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<rss version=\"2.0\" xmlns:atom=\"http://www.w3.org/2005/Atom\">");
        sb.AppendLine("<channel>");
        sb.AppendLine($"<title>RiverLi Blog</title>");
        sb.AppendLine($"<link>{baseUrl}</link>");
        sb.AppendLine($"<description>RiverLi Blog 文章订阅</description>");
        sb.AppendLine($"<atom:link href=\"{baseUrl}/api/blog/seo/rss\" rel=\"self\" type=\"application/rss+xml\"/>");

        foreach (var a in articles)
        {
            var articleUrl = $"{baseUrl}/article/{a.Slug}";
            sb.AppendLine("<item>");
            sb.AppendLine($"<title><![CDATA[{EscapeXml(a.Title)}]]></title>");
            sb.AppendLine($"<link>{articleUrl}</link>");
            sb.AppendLine($"<description><![CDATA[{EscapeXml(a.Summary)}]]></description>");
            sb.AppendLine($"<guid>{articleUrl}</guid>");
            sb.AppendLine($"<pubDate>{a.CreateTime:r}</pubDate>");
            sb.AppendLine("</item>");
        }

        sb.AppendLine("</channel>");
        sb.AppendLine("</rss>");

        return Content(sb.ToString(), "application/rss+xml; charset=utf-8");
    }

    /// <summary>Sitemap.xml</summary>
    [HttpGet("sitemap.xml")]
    [ResponseCache(Duration = 86400)]
    public async Task<IActionResult> GetSitemap(CancellationToken ct)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var articles = await _articleRepo.AsQueryable()
            .Where(a => !a.IsDeleted && a.Status == ArticleStatus.Published)
            .OrderByDescending(a => a.CreateTime)
            .Select(a => new { a.Slug, a.CreateTime })
            .ToListAsync(ct);

        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

        // 首页
        sb.AppendLine("<url>");
        sb.AppendLine($"<loc>{baseUrl}</loc>");
        sb.AppendLine($"<changefreq>daily</changefreq>");
        sb.AppendLine($"<priority>1.0</priority>");
        sb.AppendLine("</url>");

        // 友链页
        sb.AppendLine("<url>");
        sb.AppendLine($"<loc>{baseUrl}/friend</loc>");
        sb.AppendLine($"<changefreq>weekly</changefreq>");
        sb.AppendLine($"<priority>0.7</priority>");
        sb.AppendLine("</url>");

        // 文章页
        foreach (var a in articles)
        {
            sb.AppendLine("<url>");
            sb.AppendLine($"<loc>{baseUrl}/article/{a.Slug}</loc>");
            sb.AppendLine($"<lastmod>{a.CreateTime:yyyy-MM-ddTHH:mm:ss.fffZ}</lastmod>");
            sb.AppendLine($"<changefreq>weekly</changefreq>");
            sb.AppendLine($"<priority>0.9</priority>");
            sb.AppendLine("</url>");
        }

        sb.AppendLine("</urlset>");

        return Content(sb.ToString(), "application/xml; charset=utf-8");
    }

    private static string EscapeXml(string text) =>
        text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
}
