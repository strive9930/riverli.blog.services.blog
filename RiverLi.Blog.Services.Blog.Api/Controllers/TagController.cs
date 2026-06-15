using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Infrastructure.Shared.Controllers;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/blog/[controller]")]
    public class TagController : BaseApiController
    {
        private readonly IMediator _mediator;

        public TagController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>获取标签分页列表</summary>
        [HttpGet("page")]
        public async Task<IActionResult> GetPage([FromQuery] GetTagPageQuery query)
        {
            var result = await _mediator.Send(query);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>新增标签</summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTagCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>修改标签信息</summary>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTagCommand command)
        {
            if (id != command.Id)
                return BadRequest("路由 ID 与请求体 ID 不一致");

            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>删除标签 (软删除)</summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteTagCommand(id));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }
    }
}
