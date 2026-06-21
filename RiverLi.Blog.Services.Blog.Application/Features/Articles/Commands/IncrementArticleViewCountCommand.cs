using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

public record IncrementArticleViewCountCommand(Guid Id) : IRequest<Result>;
