using System;
using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates;

/// <summary>
/// 站点导航聚合根 (用于博客前台顶部菜单展示)
/// </summary>
public class SiteNavigation : BaseEntity<Guid>, IAggregateRoot
{
    /// <summary>
    /// 导航显示名称
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// 跳转链接 (可以是相对路径 /categories，也可以是绝对路径 https://github.com...)
    /// </summary>
    public string LinkUrl { get; private set; }

    /// <summary>
    /// 图标 (可选，支持前端的 Iconify 或 Element Plus 图标名)
    /// </summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// 排序权重 (数字越小越靠前)
    /// </summary>
    public int SortOrder { get; private set; }

    /// <summary>
    /// 打开目标 (例如：_self 当前窗口, _blank 新窗口)
    /// </summary>
    public string Target { get; private set; }

    /// <summary>
    /// 是否可见 (支持后台一键隐藏某个菜单项)
    /// </summary>
    public bool IsVisible { get; private set; }

    // 给 EF Core 预留的无参构造
    private SiteNavigation() { }

    public SiteNavigation(string title, string linkUrl, string? icon, int sortOrder, string target = "_self", bool isVisible = true)
    {
        Title = title;
        LinkUrl = linkUrl;
        Icon = icon;
        SortOrder = sortOrder;
        Target = target;
        IsVisible = isVisible;
    }

    /// <summary>
    /// 领域行为：更新导航信息
    /// </summary>
    public void Update(string title, string linkUrl, string? icon, int sortOrder, string target, bool isVisible)
    {
        Title = title;
        LinkUrl = linkUrl;
        Icon = icon;
        SortOrder = sortOrder;
        Target = target;
        IsVisible = isVisible;

        UpdateModifyTime();
    }

    /// <summary>
    /// 领域行为：切换可见状态
    /// </summary>
    public void ToggleVisibility()
    {
        IsVisible = !IsVisible;
        UpdateModifyTime();
    }
}