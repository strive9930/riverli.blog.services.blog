using System.Collections.Generic;
using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Queries;

/// <summary>
/// 获取全局最新 N 条已审核评论
/// </summary>
public record GetLatestCommentsQuery(int Count = 5) : IRequest<Result<List<CommentDto>>>;
