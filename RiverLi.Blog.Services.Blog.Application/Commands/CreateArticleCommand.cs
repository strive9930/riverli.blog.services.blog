using System;
using System.Collections.Generic;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models; // 假设您的 Result 位于此处

namespace RiverLi.Blog.Services.Blog.Application.Commands;

/// <summary>
/// 创建文章的命令请求 (不含作者信息，作者信息将在 Handler 中通过当前登录的 JWT Token 自动解析)
/// </summary>
/// <param name="Title">文章标题</param>
/// <param name="Content">Markdown 核心正文</param>
/// <param name="Summary">文章摘要描述</param>
/// <param name="CoverUrl">封面图 URL（允许为空）</param>
/// <param name="CategoryId">所属分类 ID</param>
/// <param name="TagIds">关联的标签 ID 集合（允许为空）</param>
public record CreateArticleCommand(
    string Title,
    string Content,
    string Summary,
    string? CoverUrl,
    Guid CategoryId,
    List<Guid>? TagIds
) : IRequest<Result<Guid>>; // 规定该写操作执行完毕后，向 API 层返回新文章的 Guid