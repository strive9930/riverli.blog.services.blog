namespace RiverLi.Blog.Services.Blog.Application.Common.Dto
{
    public record WallDto(Guid Id, string Content, string Email, string Name, string? Url,
        string? Avatar, int CateId, string Status, DateTime CreateTime);

    public record WallCateDto(Guid Id, string Name, string? Icon, string? Color, DateTime CreateTime);
}
