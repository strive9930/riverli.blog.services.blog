using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Records.Commands;

public record CreateRecordCommand(string Content, string? ImageUrls, string? Location, bool IsPublic = true) : IRequest<Result<Guid>>;

public record UpdateRecordCommand(Guid Id, string Content, string? ImageUrls, string? Location, bool IsPublic) : IRequest<Result>;

public record DeleteRecordCommand(Guid Id) : IRequest<Result>;
