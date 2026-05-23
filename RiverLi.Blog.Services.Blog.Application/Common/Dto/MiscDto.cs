namespace RiverLi.Blog.Services.Blog.Application.Common.Dto
{
    public record SwiperDto(Guid Id, string Image, string? Title, string? Description,
        string? Url, int Sort, bool IsActive, DateTime CreateTime);

    public record FootprintDto(Guid Id, DateTime Time, string Location, string Content,
        string Images, decimal? Longitude, decimal? Latitude, DateTime CreateTime);

    public record RecordDto(Guid Id, string Content, string Images, DateTime CreateTime);

    public record RssDto(Guid Id, string Name, string Url, string? Logo, string? Description,
        DateTime? LastFetchTime, bool IsActive, DateTime CreateTime);

    public record ConfigDto(Guid Id, string Name, string Value, string? Notes, DateTime CreateTime);

    public record EnvConfigDto(Guid Id, string Name, string Value, DateTime CreateTime);

    public record PageConfigDto(Guid Id, string Name, string Config, DateTime CreateTime);

    public record OssDto(Guid Id, string Platform, string AccessKey, string SecretKey,
        string Bucket, string Domain, string BasePath, bool IsDefault, DateTime CreateTime);

    public record FileDetailDto(Guid Id, string FileName, string FilePath, string FileUrl,
        long FileSize, string FileType, string Platform, DateTime CreateTime);
}
