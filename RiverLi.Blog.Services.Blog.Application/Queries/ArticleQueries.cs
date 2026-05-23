using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using Microsoft.EntityFrameworkCore;

namespace RiverLi.Blog.Services.Blog.Application.Queries
{
    public record GetArticleQuery(Guid Id, string? Password = null) : IRequest<Result<ArticleDto>>;
    public record GetArticleListQuery(string? Keyword = null, int? CateId = null, int? TagId = null,
        string? Status = null, string? SortBy = null, string? SortOrder = null) : IRequest<Result<List<ArticleListDto>>>;
    public class GetArticlePagingQuery : PagedQuery, IRequest<PagedResult<ArticleListDto>>
    {
        public string? Keyword { get; set; }
        public int? CateId { get; set; }
        public int? TagId { get; set; }
        public string? Status { get; set; }
    }
    public record GetRandomArticlesQuery(int Count = 5) : IRequest<Result<List<ArticleListDto>>>;
    public record GetArticleArchiveQuery : IRequest<Result<Dictionary<string, int>>>;
    public record SearchArticlesQuery(string Keyword) : IRequest<Result<List<ArticleListDto>>>;

    public class ArticleQueryHandler :
        IRequestHandler<GetArticleQuery, Result<ArticleDto>>,
        IRequestHandler<GetArticleListQuery, Result<List<ArticleListDto>>>,
        IRequestHandler<GetArticlePagingQuery, PagedResult<ArticleListDto>>,
        IRequestHandler<GetRandomArticlesQuery, Result<List<ArticleListDto>>>,
        IRequestHandler<GetArticleArchiveQuery, Result<Dictionary<string, int>>>,
        IRequestHandler<SearchArticlesQuery, Result<List<ArticleListDto>>>
    {
        private readonly IRepository<Article, Guid> _repository;

        public ArticleQueryHandler(IRepository<Article, Guid> repository) => _repository = repository;

        public async Task<Result<ArticleDto>> Handle(GetArticleQuery request, CancellationToken ct)
        {
            var article = await _repository.GetByIdAsync(request.Id, ct);
            if (article == null) return Result<ArticleDto>.FailResult("文章不存在", 404);
            var dto = MapDetail(article);
            return Result<ArticleDto>.SuccessResult(dto);
        }

        public async Task<Result<List<ArticleListDto>>> Handle(GetArticleListQuery request, CancellationToken ct)
        {
            var query = _repository.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Keyword))
                query = query.Where(a => a.Title.Contains(request.Keyword) || a.Content.Contains(request.Keyword));
            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(a => a.Config.Contains(request.Status));
            query = query.OrderByDescending(a => a.CreateTime);
            var list = await query.Take(50).ToListAsync(ct);
            return Result<List<ArticleListDto>>.SuccessResult(list.Select(MapList).ToList());
        }

        public async Task<PagedResult<ArticleListDto>> Handle(GetArticlePagingQuery request, CancellationToken ct)
        {
            var query = _repository.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Keyword))
                query = query.Where(a => a.Title.Contains(request.Keyword) || a.Content.Contains(request.Keyword));
            if (!string.IsNullOrWhiteSpace(request.Status))
                query = query.Where(a => a.Config.Contains(request.Status));
            query = query.OrderByDescending(a => a.CreateTime);

            var total = await query.CountAsync(ct);
            var items = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize).ToListAsync(ct);

            return PagedResult<ArticleListDto>.SuccessResult(
                items.Select(MapList).ToList(), total,
                request.PageIndex, request.PageSize);
        }

        public async Task<Result<List<ArticleListDto>>> Handle(GetRandomArticlesQuery request, CancellationToken ct)
        {
            var all = await _repository.GetAllAsync(ct);
            var random = all.OrderBy(_ => Guid.NewGuid()).Take(request.Count).ToList();
            return Result<List<ArticleListDto>>.SuccessResult(random.Select(MapList).ToList());
        }

        public async Task<Result<Dictionary<string, int>>> Handle(GetArticleArchiveQuery request, CancellationToken ct)
        {
            var all = await _repository.GetAllAsync(ct);
            var archive = all.GroupBy(a => a.CreateTime.ToString("yyyy年MM月"))
                .ToDictionary(g => g.Key, g => g.Count());
            return Result<Dictionary<string, int>>.SuccessResult(archive);
        }

        public async Task<Result<List<ArticleListDto>>> Handle(SearchArticlesQuery request, CancellationToken ct)
        {
            var query = _repository.AsQueryable()
                .Where(a => a.Title.Contains(request.Keyword) || a.Content.Contains(request.Keyword))
                .OrderByDescending(a => a.CreateTime)
                .Take(20);
            var list = await query.ToListAsync(ct);
            return Result<List<ArticleListDto>>.SuccessResult(list.Select(MapList).ToList());
        }

        private static ArticleDto MapDetail(Article a) => new(
            a.Id, a.Title, a.Content, a.Description, a.Cover, a.Config,
            a.ViewCount, a.AuthorId, a.AuthorName, a.CreateTime)
        {
            TagNames = a.Tags.Select(t => t.TagName).ToList()
        };

        private static ArticleListDto MapList(Article a) => new(
            a.Id, a.Title, a.Description, a.Cover, a.ViewCount, a.AuthorName, a.CreateTime);
    }
}
