using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Tags.Commands;

/// <summary>
/// 删除标签命令 (软删�?
/// </summary>
public record DeleteTagCommand(Guid Id) : IRequest<Result>;
