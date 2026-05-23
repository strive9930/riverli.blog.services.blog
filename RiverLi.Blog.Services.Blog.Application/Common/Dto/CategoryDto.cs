namespace RiverLi.Blog.Services.Blog.Application.Common.Dto
{
    public record CategoryDto(Guid Id, string Name, string? Icon, Guid? ParentId, DateTime CreateTime)
    {
        public List<CategoryDto>? Children { get; set; }
        public int ArticleCount { get; set; }
    }
}
