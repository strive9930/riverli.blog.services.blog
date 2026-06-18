using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using RiverLi.Blog.Services.Blog.Application.Queries;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Api.Controllers;

/// <summary>
/// 博客前台站点导航管理
/// </summary>
[ApiController]
[Route("api/blog/[controller]")]
[Authorize] // 默认全控制器需要登录（保护后台写入接口）
public class SiteNavigationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SiteNavigationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// 获取前台导航列表 (公开接口，无需登录，自动过滤隐藏项)
    /// </summary>
    [HttpGet]
    [AllowAnonymous] // 🌟 极其重要：对前台游客彻底放行
    public async Task<ActionResult<Result<List<SiteNavigationDto>>>> GetForFrontend()
    {
        var result = await _mediator.Send(new GetSiteNavigationsQuery(IsAdminView: false));
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>
    /// 获取后台导航列表 (需要登录，包含隐藏项)
    /// </summary>
    [HttpGet("admin")]
    public async Task<ActionResult<Result<List<SiteNavigationDto>>>> GetForAdmin()
    {
        var result = await _mediator.Send(new GetSiteNavigationsQuery(IsAdminView: true));
        return  result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>
    /// 新增站点导航 (仅限管理员)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Result<Guid>>> Create([FromBody] CreateSiteNavigationCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Success) return BadRequest(result.Message);
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }
    
    /// <summary>
    /// 更新站点导航 (仅限管理员)
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Result<bool>>> Update(Guid id, [FromBody] UpdateSiteNavigationCommand command)
    {
        // 防御性编程：防止前端恶意篡改 URL 中的 ID 和 Body 中的 ID 不一致
        if (id != command.Id)
        {
            return BadRequest(Result<bool>.FailResult("路由中的ID与提交的表单ID不一致"));
        }

        var result = await _mediator.Send(command);
        
        if (!result.Success) return BadRequest(result);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// 删除站点导航 (仅限管理员)
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Result<bool>>> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteSiteNavigationCommand(id));
        
        if (!result.Success) return BadRequest(result);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}