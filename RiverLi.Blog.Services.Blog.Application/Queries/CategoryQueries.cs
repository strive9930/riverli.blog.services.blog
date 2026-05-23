using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;

namespace RiverLi.Blog.Services.Blog.Application.Queries
{
    public record GetCategoryQuery(Guid Id) : IRequest<Result<CategoryDto>>;
    public record GetCategoryListQuery : IRequest<Result<List<CategoryDto>>>;
    public class GetCategoryPagingQuery : PagedQuery, IRequest<PagedResult<CategoryDto>>;
    public record GetCategoryTreeQuery : IRequest<Result<List<CategoryDto>>>;
    public record GetCategoryArticleCountQuery : IRequest<Result<List<CategoryDto>>>;

    public class CategoryQueryHandler :
        IRequestHandler<GetCategoryQuery, Result<CategoryDto>>,
        IRequestHandler<GetCategoryListQuery, Result<List<CategoryDto>>>,
        IRequestHandler<GetCategoryPagingQuery, PagedResult<CategoryDto>>,
        IRequestHandler<GetCategoryTreeQuery, Result<List<CategoryDto>>>,
        IRequestHandler<GetCategoryArticleCountQuery, Result<List<CategoryDto>>>
    {
        private readonly IRepository<Category, Guid> _repository;

        public CategoryQueryHandler(IRepository<Category, Guid> repository) => _repository = repository;

        public async Task<Result<CategoryDto>> Handle(GetCategoryQuery request, CancellationToken ct)
        {
            var cat = await _repository.GetByIdAsync(request.Id, ct);
            if (cat == null) return Result<CategoryDto>.FailResult("分类不存在", 404);
            return Result<CategoryDto>.SuccessResult(Map(cat));
        }

        public async Task<Result<List<CategoryDto>>> Handle(GetCategoryListQuery request, CancellationToken ct)
        {
            var list = await _repository.GetAllAsync(ct);
            return Result<List<CategoryDto>>.SuccessResult(list.Select(Map).ToList());
        }

        public async Task<PagedResult<CategoryDto>> Handle(GetCategoryPagingQuery request, CancellationToken ct)
        {
            var paged = await _repository.GetPagedAsync(request, null, ct);
            return PagedResult<CategoryDto>.SuccessResult(
                paged.Data.Select(Map).ToList(), paged.TotalCount,
                paged.PageIndex, paged.PageSize);
        }

        public async Task<Result<List<CategoryDto>>> Handle(GetCategoryTreeQuery request, CancellationToken ct)
        {
            var all = await _repository.GetAllAsync(ct);
            var topLevel = all.Where(c => c.ParentId == null).Select(Map).ToList();
            BuildTree(all, topLevel);
            return Result<List<CategoryDto>>.SuccessResult(topLevel);
        }

        public async Task<Result<List<CategoryDto>>> Handle(GetCategoryArticleCountQuery request, CancellationToken ct)
        {
            var all = await _repository.GetAllAsync(ct);
            return Result<List<CategoryDto>>.SuccessResult(all.Select(Map).ToList());
        }

        private void BuildTree(List<Category> all, List<CategoryDto> parents)
        {
            foreach (var p in parents)
            {
                var children = all.Where(c => c.ParentId == p.Id).ToList();
                p.Children = children.Select(Map).ToList();
                BuildTree(all, p.Children);
            }
        }

        private static CategoryDto Map(Category c) =>
            new(c.Id, c.Name, c.Icon, c.ParentId, c.CreateTime);
    }
}
