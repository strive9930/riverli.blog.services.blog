namespace RiverLi.Blog.Services.Blog.Application.Common.Dto
{
    public record CommentDto(
        Guid Id,
        Guid? ArticleId,
        string Content,
        string Email,
        string Name,
        string? Url,
        string? Avatar,
        Guid ParentId,
        string Status,
        DateTime CreateTime)
    {
        public List<CommentDto>? Replies { get; set; }
    }
}
