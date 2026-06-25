using System;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Enum;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

/// <summary>
/// 切换文章状态命令 (上架/下架)
/// </summary>
/// <param name="Id">文章 ID</param>
/// <param name="Status">目标状态</param>
/// <param name="IsSystem">系统内部调用（定时任务等无登录用户场景），跳过权限校验</param>
public record ChangeArticleStatusCommand(Guid Id, ArticleStatus Status, bool IsSystem = false) : IRequest<Result>;
