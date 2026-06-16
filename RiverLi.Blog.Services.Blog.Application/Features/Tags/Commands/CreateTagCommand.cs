using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Tags.Commands;

/// <summary>
/// 新增标签命令
/// </summary>
public record CreateTagCommand(string Name, string Slug) : IRequest<Result<Guid>>;
