using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiverLi.Blog.Services.Blog.Application.Commands
{
    public record CreateArticleCommand(string Title, string Content, string Summary, string CoverUrl, List<string> Tags) : IRequest<Result<Guid>>;
}
