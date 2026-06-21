using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Infrastructure.Shared.Controllers;
using RiverLi.Blog.Services.Blog.Application.Features.Records.Commands;
using RiverLi.Blog.Services.Blog.Application.Features.Records.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers;

[ApiController]
[Route("api/blog/[controller]")]
public class RecordController : BaseApiController
{
    private readonly IMediator _mediator;

    public RecordController(IMediator mediator) => _mediator = mediator;

    /// <summary>公开：获取公开动态列表</summary>
    [AllowAnonymous]
    [HttpGet("page")]
    public async Task<IActionResult> GetPublic([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetRecordPageQuery(pageIndex, pageSize, AdminView: false));
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>后台：获取所有动态（含私密）</summary>
    [AllowAnonymous] // TODO: 临时放开，后续改为 [Authorize]
    [HttpGet("admin/page")]
    public async Task<IActionResult> GetAdmin([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetRecordPageQuery(pageIndex, pageSize, AdminView: true));
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>发布动态（需登录）</summary>
    [AllowAnonymous] // TODO: 临时放开，后续改为 [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRecordCommand command)
    {
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>编辑动态（需登录）</summary>
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRecordCommand command)
    {
        if (id != command.Id) return BadRequest("路由 ID 与请求体 ID 不一致");
        var result = await _mediator.Send(command);
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    /// <summary>删除动态（需登录）</summary>
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteRecordCommand(id));
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }
}
