using System;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public record DeleteSiteNavigationCommand(Guid Id) : IRequest<Result<bool>>;