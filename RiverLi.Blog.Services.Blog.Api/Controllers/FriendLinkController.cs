using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Infrastructure.Shared.Controllers;
using RiverLi.Blog.Services.Blog.Application.Features.FriendLinks.Commands;
using RiverLi.Blog.Services.Blog.Application.Features.FriendLinks.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers;

[ApiController]
[Route("api/blog/[controller]")]
public class FriendLinkController : BaseApiController
{
    private readonly IMediator _mediator;

    public FriendLinkController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>公开：获取已审核友链列表</summary>
    [AllowAnonymous]
    [HttpGet("page")]
    public async Task<IActionResult> GetPublic([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetFriendLinkPageQuery(pageIndex, pageSize, AdminView: false));
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>后台：获取所有友链（含待审核）</summary>
    [AllowAnonymous] // TODO: 临时放开，后续改为 [Authorize]
    [HttpGet("admin/page")]
    public async Task<IActionResult> GetAdmin([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetFriendLinkPageQuery(pageIndex, pageSize, AdminView: true));
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>申请友链（公开）</summary>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Apply([FromBody] CreateFriendLinkCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>修改友链信息（需登录）</summary>
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFriendLinkCommand command)
    {
        if (id != command.Id) return BadRequest("路由 ID 与请求体 ID 不一致");
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>审核友链（需登录）</summary>
    [Authorize]
    [HttpPut("{id:guid}/audit")]
    public async Task<IActionResult> Audit(Guid id, [FromBody] AuditFriendLinkCommand command)
    {
        if (id != command.Id) return BadRequest("路由 ID 与请求体 ID 不一致");
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>切换置顶（需登录）</summary>
    [Authorize]
    [HttpPut("{id:guid}/top")]
    public async Task<IActionResult> ToggleTop(Guid id)
    {
        var result = await _mediator.Send(new ToggleFriendLinkTopCommand(id));
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>删除友链（需登录）</summary>
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteFriendLinkCommand(id));
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }
}
