using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using Microsoft.EntityFrameworkCore;

namespace RiverLi.Blog.Services.Blog.Application.Queries
{
    public record GetTagQuery(Guid Id) : IRequest<Result<TagDto>>;
    public record GetTagListQuery : IRequest<Result<List<TagDto>>>;
    public class GetTagPagingQuery : PagedQuery, IRequest<PagedResult<TagDto>>;
    public record GetHotTagsQuery(int Count = 10) : IRequest<Result<List<TagDto>>>;

    public class TagQueryHandler :
        IRequestHandler<GetTagQuery, Result<TagDto>>,
        IRequestHandler<GetTagListQuery, Result<List<TagDto>>>,
        IRequestHandler<GetTagPagingQuery, PagedResult<TagDto>>,
        IRequestHandler<GetHotTagsQuery, Result<List<TagDto>>>
    {
        private readonly IRepository<Tag, Guid> _repository;

        public TagQueryHandler(IRepository<Tag, Guid> repository) => _repository = repository;

        public async Task<Result<TagDto>> Handle(GetTagQuery request, CancellationToken ct)
        {
            var tag = await _repository.GetByIdAsync(request.Id, ct);
            if (tag == null) return Result<TagDto>.FailResult("标签不存在", 404);
            return Result<TagDto>.SuccessResult(Map(tag));
        }

        public async Task<Result<List<TagDto>>> Handle(GetTagListQuery request, CancellationToken ct)
        {
            var list = await _repository.GetAllAsync(ct);
            return Result<List<TagDto>>.SuccessResult(list.Select(Map).ToList());
        }

        public async Task<PagedResult<TagDto>> Handle(GetTagPagingQuery request, CancellationToken ct)
        {
            var paged = await _repository.GetPagedAsync(request, null, ct);
            return PagedResult<TagDto>.SuccessResult(
                paged.Data.Select(Map).ToList(), paged.TotalCount,
                paged.PageIndex, paged.PageSize);
        }

        public async Task<Result<List<TagDto>>> Handle(GetHotTagsQuery request, CancellationToken ct)
        {
            var list = await _repository.GetAllAsync(ct);
            return Result<List<TagDto>>.SuccessResult(list.Take(request.Count).Select(Map).ToList());
        }

        private static TagDto Map(Tag t) =>
            new(t.Id, t.Name, t.Color, t.CreateTime);
    }
}
