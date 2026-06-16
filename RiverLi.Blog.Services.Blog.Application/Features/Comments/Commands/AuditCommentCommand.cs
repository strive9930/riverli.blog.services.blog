using System;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Comments.Commands;

/// <summary>
/// 审核评论命令 (通过/拒绝)
/// </summary>
public record AuditCommentCommand(Guid Id, CommentStatus Status) : IRequest<Result>;
