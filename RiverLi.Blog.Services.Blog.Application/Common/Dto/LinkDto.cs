namespace RiverLi.Blog.Services.Blog.Application.Common.Dto
{
    public record LinkDto(Guid Id, string Name, string Url, string? Logo, string? Description,
        int TypeId, string Status, string? Email, DateTime CreateTime);

    public record LinkTypeDto(Guid Id, string Name, string? Icon, DateTime CreateTime);
}
