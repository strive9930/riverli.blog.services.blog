using System;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.FriendLinks.Commands;

public record AuditFriendLinkCommand(Guid Id, FriendLinkStatus Status) : IRequest<Result>;
