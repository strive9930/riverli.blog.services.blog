using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Infrastructure.Shared.Controllers;
using RiverLi.Blog.Services.Blog.Application.Features.Messages.Commands;
using RiverLi.Blog.Services.Blog.Application.Features.Messages.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers;

[ApiController]
[Route("api/blog/[controller]")]
public class MessageController : BaseApiController
{
    private readonly IMediator _mediator;

    public MessageController(IMediator mediator) => _mediator = mediator;

    /// <summary>公开：获取已审核留言</summary>
    [AllowAnonymous]
    [HttpGet("page")]
    public async Task<IActionResult> GetPublic([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetMessagePageQuery(pageIndex, pageSize, AdminView: false));
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>公开：提交留言</summary>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMessageCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>后台：获取所有留言</summary>
    [AllowAnonymous] // TODO: 临时放开，后续改为 [Authorize]
    [HttpGet("admin")]
    public async Task<IActionResult> GetAdmin([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetMessagePageQuery(pageIndex, pageSize, AdminView: true));
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>审核留言（需登录）</summary>
    [Authorize]
    [HttpPut("{id:guid}/audit")]
    public async Task<IActionResult> Audit(Guid id, [FromBody] AuditMessageCommand command)
    {
        if (id != command.Id) return BadRequest("路由 ID 与请求体 ID 不一致");
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>删除留言（需登录）</summary>
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteMessageCommand(id));
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }
}
