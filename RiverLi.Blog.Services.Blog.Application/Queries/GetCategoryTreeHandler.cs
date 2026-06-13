using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.Blog.Services.Blog.Infrastructure.Data;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Queries;

public class GetCategoryTreeHandler : IRequestHandler<GetCategoryTreeQuery, Result<List<CategoryDto>>>
{
    private readonly BlogDbContext _dbContext;

    public GetCategoryTreeHandler(BlogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<CategoryDto>>> Handle(GetCategoryTreeQuery request, CancellationToken cancellationToken)
    {
        // 查出所有未删除的分类
        var categories = await _dbContext.Categories
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.SortOrder)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.SortOrder,
                c.ParentId,
                ArticleCount = _dbContext.Articles.Count(a => a.CategoryId == c.Id && !a.IsDeleted)
            })
            .ToListAsync(cancellationToken);

        // 内存中构建树形结构
        var dtoMap = categories.ToDictionary(
            c => c.Id,
            c => new CategoryDto(c.Id, c.Name, c.Slug, c.Description, c.SortOrder, c.ArticleCount, new List<CategoryDto>())
        );

        var roots = new List<CategoryDto>();
        foreach (var cat in categories)
        {
            var dto = dtoMap[cat.Id];
            if (cat.ParentId.HasValue && dtoMap.ContainsKey(cat.ParentId.Value))
            {
                dtoMap[cat.ParentId.Value].Children.Add(dto);
            }
            else
            {
                roots.Add(dto);
            }
        }

        return Result<List<CategoryDto>>.SuccessResult(roots);
    }
}
