namespace RiverLi.Blog.Services.Blog.Application.Common.Dto
{
    public record TagDto(Guid Id, string Name, string? Color, DateTime CreateTime)
    {
        public int ArticleCount { get; set; }
    }
}
