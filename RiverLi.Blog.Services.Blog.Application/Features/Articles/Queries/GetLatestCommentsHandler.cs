using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

public class GetLatestCommentsHandler : IRequestHandler<GetLatestCommentsQuery, Result<List<CommentDto>>>
{
    private readonly IRepository<Comment, Guid> _commentRepo;
    private readonly IRepository<Article, Guid> _articleRepo;

    public GetLatestCommentsHandler(IRepository<Comment, Guid> commentRepo, IRepository<Article, Guid> articleRepo)
    {
        _commentRepo = commentRepo;
        _articleRepo = articleRepo;
    }

    public async Task<Result<List<CommentDto>>> Handle(GetLatestCommentsQuery request, CancellationToken cancellationToken)
    {
        var items = await _commentRepo.AsQueryable()
            .Where(c => c.Status == CommentStatus.Approved)
            .OrderByDescending(c => c.CreateTime)
            .Take(request.Count)
            .Select(c => new CommentDto(
                c.Id, c.ArticleId,
                _articleRepo.AsQueryable().Where(a => a.Id == c.ArticleId).Select(a => a.Title).FirstOrDefault() ?? "",
                c.ReviewerName, c.Content, c.Status.ToString(), c.CreateTime
            ))
            .ToListAsync(cancellationToken);

        return Result<List<CommentDto>>.SuccessResult(items);
    }
}
