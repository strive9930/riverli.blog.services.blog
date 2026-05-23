using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Commands
{
    public record CreateArticleCommand(string Title, string Content, string? Description,
        string? Cover, string Config, List<string> Tags) : IRequest<Result<Guid>>;
}
