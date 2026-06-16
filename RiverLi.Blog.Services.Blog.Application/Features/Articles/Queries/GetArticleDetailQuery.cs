using System;
using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

/// <summary>
/// 获取单篇文章详情
/// </summary>
public record GetArticleDetailQuery(Guid Id) : IRequest<Result<ArticleDetailDto>>;
