using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

/// <summary>
/// 删除标签命令 (软删除)
/// </summary>
public record DeleteTagCommand(Guid Id) : IRequest<Result>;
