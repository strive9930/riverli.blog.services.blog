using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Medias.Commands;

/// <summary>
/// 删除媒体文件命令 (物理删除)
/// </summary>
public record DeleteMediaCommand(Guid Id) : IRequest<Result>;
