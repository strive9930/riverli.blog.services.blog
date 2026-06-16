using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Comments.Commands;

/// <summary>
/// 读者提交评论命�?(无需登录态，由网关处�?
/// </summary>
public record CreateCommentCommand(Guid ArticleId, string Content) : IRequest<Result<Guid>>;
