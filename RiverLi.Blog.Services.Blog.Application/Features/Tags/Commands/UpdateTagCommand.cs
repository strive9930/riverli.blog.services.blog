using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Tags.Commands;

/// <summary>
/// 修改标签命令
/// </summary>
public record UpdateTagCommand(Guid Id, string Name, string Slug) : IRequest<Result>;
