using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

/// <summary>
/// 删除文章命令 (软删�?
/// </summary>
public record DeleteArticleCommand(Guid Id) : IRequest<Result>;
