using System;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

/// <summary>
/// 切换文章状态命�?(上架/下架)
/// </summary>
public record ChangeArticleStatusCommand(Guid Id, ArticleStatus Status) : IRequest<Result>;
