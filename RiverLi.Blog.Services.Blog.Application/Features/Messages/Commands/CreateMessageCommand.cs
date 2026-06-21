using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Messages.Commands;

public record CreateMessageCommand(string Nickname, string Content, string? Contact) : IRequest<Result<Guid>>;
