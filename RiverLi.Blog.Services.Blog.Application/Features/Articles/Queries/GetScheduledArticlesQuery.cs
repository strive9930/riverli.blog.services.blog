using System;
using System.Collections.Generic;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

/// <summary>
/// 查询所有到期的定时发布文章 ID
/// </summary>
public record GetScheduledArticlesQuery() : IRequest<Result<List<Guid>>>;
