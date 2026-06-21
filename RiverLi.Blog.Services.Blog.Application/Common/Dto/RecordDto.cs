using System;

namespace RiverLi.Blog.Services.Blog.Application.Common.Dto;

public record RecordDto(
    Guid Id,
    string Content,
    string? ImageUrls,
    string? Location,
    bool IsPublic,
    DateTime CreatedTime
);
