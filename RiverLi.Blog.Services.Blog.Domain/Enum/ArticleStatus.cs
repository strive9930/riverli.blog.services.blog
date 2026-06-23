using System.Text.Json.Serialization;

namespace RiverLi.Blog.Services.Blog.Domain.Enum;

/// <summary>
/// 文章发布状态
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ArticleStatus 
{ 
    Draft = 0,      // 草稿
    Published = 1,  // 已发布
    Scheduled = 2   // 待定时发布
}
