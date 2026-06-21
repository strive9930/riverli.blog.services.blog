using System;

namespace RiverLi.Blog.Services.Blog.Application.Common.Dto;

public record MessageDto(
    Guid Id,
    string Nickname,
    string Content,
    string? Contact,
    string Status,
    DateTime CreatedTime
);
