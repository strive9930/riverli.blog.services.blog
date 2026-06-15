using System.Collections.Generic;
using MediatR;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Queries;

/// <summary>
/// 获取分类扁平选项列表 (供下拉框使用)
/// </summary>
public record GetCategoryOptionsQuery : IRequest<Result<List<CategoryOptionDto>>>;
