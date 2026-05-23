namespace RiverLi.Blog.Services.Blog.Application.Common.Dto
{
    public record AlbumCateDto(Guid Id, string Name, string? Description, string? Cover, DateTime CreateTime);

    public record AlbumImageDto(Guid Id, int CateId, string Url, string? Thumbnail,
        string? Description, int Size, int Width, int Height, DateTime CreateTime);
}
