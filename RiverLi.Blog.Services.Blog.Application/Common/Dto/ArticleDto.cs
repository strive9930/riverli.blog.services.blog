namespace RiverLi.Blog.Services.Blog.Application.Common.Dto
{
    public record ArticleDto(Guid Id, string Title, string Content, string? Description,
        string? Cover, string Config, int ViewCount, string AuthorId, string AuthorName,
        DateTime CreateTime)
    {
        public List<string>? CateNames { get; set; }
        public List<string>? TagNames { get; set; }
        public object? Prev { get; set; }
        public object? Next { get; set; }
    }

    public record ArticleListDto(Guid Id, string Title, string? Description, string? Cover,
        int ViewCount, string AuthorName, DateTime CreateTime);
}
