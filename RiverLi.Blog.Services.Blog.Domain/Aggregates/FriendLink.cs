using System;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates;

/// <summary>
/// 友链聚合根
/// </summary>
public class FriendLink : BaseEntity<Guid>, IAggregateRoot
{
    public string SiteName { get; private set; }
    public string SiteDescription { get; private set; }
    public string Url { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string Owner { get; private set; }
    public string? RssUrl { get; private set; }
    public FriendLinkStatus Status { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsTop { get; private set; } // 全站置顶

    private FriendLink() { }

    public FriendLink(string siteName, string siteDescription, string url, string? avatarUrl, string owner, string? rssUrl)
    {
        SiteName = siteName;
        SiteDescription = siteDescription;
        Url = url;
        AvatarUrl = avatarUrl;
        Owner = owner;
        RssUrl = rssUrl;
        Status = FriendLinkStatus.Pending;
        SortOrder = 0;
        IsTop = false;
    }

    public void Update(string siteName, string siteDescription, string url, string? avatarUrl, string owner, string? rssUrl)
    {
        SiteName = siteName;
        SiteDescription = siteDescription;
        Url = url;
        AvatarUrl = avatarUrl;
        Owner = owner;
        RssUrl = rssUrl;
        UpdateModifyTime();
    }

    public void Audit(FriendLinkStatus status)
    {
        Status = status;
        UpdateModifyTime();
    }

    public void ToggleTop()
    {
        IsTop = !IsTop;
        UpdateModifyTime();
    }
}
