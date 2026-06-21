using System;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Messages.Commands;

public record AuditMessageCommand(Guid Id, CommentStatus Status) : IRequest<Result>;

public record DeleteMessageCommand(Guid Id) : IRequest<Result>;
