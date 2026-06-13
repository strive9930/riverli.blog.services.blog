using System.Collections.Generic;
using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Queries;

/// <summary>
/// 获取分类树形结构
/// </summary>
public record GetCategoryTreeQuery : IRequest<Result<List<CategoryDto>>>;
