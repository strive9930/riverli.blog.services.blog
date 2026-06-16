using System.Text.Json.Serialization;

namespace RiverLi.Blog.Services.Blog.Domain.Enum;

/// <summary>
/// 评论审核状态
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CommentStatus 
{ 
    Pending = 0,   // 待审核
    Approved = 1,  // 已通过
    Rejected = 2   // 已拒绝
}