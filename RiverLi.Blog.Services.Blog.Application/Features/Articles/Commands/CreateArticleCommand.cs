using System;
using System.Collections.Generic;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

/// <summary>
/// 创建文章的命令请求 (不含作者信息，作者信息将在 Handler 中通过当前登录的 JWT Token 自动解析)
/// </summary>
/// <param name="Title">文章标题</param>
/// <param name="Slug">URL 友好标识（可选，留空则根据标题自动生成）</param>
/// <param name="Content">Markdown 核心正文</param>
/// <param name="Summary">文章摘要描述</param>
/// <param name="CoverUrl">封面图 URL（允许为空）</param>
/// <param name="CategoryId">所属分类 ID</param>
/// <param name="TagIds">关联的标签 ID 集合（允许为空）</param>
/// <param name="ScheduledPublishTime">定时发布时间（可选，留空则立即可发布）</param>
public record CreateArticleCommand(
    string Title,
    string? Slug,
    string Content,
    string Summary,
    string? CoverUrl,
    Guid CategoryId,
    List<Guid>? TagIds,
    DateTime? ScheduledPublishTime
) : IRequest<Result<Guid>>;
