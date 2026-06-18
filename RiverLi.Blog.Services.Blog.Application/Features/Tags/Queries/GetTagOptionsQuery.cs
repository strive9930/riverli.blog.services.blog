using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Features.Tags.Queries;

public record GetTagOptionsQuery() : IRequest<Result<List<TagOptionDto>>>;