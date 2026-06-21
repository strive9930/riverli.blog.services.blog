using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.FriendLinks.Commands;

public record ToggleFriendLinkTopCommand(Guid Id) : IRequest<Result>;
