using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Comments.Commands;

/// <summary>
/// 彻底删除评论命令
/// </summary>
public record DeleteCommentCommand(Guid Id) : IRequest<Result>;
