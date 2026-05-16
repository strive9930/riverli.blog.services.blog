using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiverLi.Blog.Services.Blog.Application.Common.Dto
{
    public record ArticleDto(Guid Id, string Title, string AuthorName, DateTime CreatedTime, int ViewCount);
}
