using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Infrastructure.Shared.Controllers;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/blog/[controller]")]
    public class CommentController : BaseApiController
    {
        private readonly IMediator _mediator;

        public CommentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>获取评论分页列表 (后台审核用)</summary>
        [Authorize]
        [HttpGet("page")]
        public async Task<IActionResult> GetPage([FromQuery] GetCommentPageQuery query)
        {
            var result = await _mediator.Send(query);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>读者提交评论 (前台接口，需登录)</summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>审核评论 (通过/拒绝)</summary>
        [Authorize]
        [HttpPut("{id:guid}/audit")]
        public async Task<IActionResult> Audit(Guid id, [FromBody] AuditCommentCommand command)
        {
            if (id != command.Id)
                return BadRequest("路由 ID 与请求体 ID 不一致");

            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>彻底删除违规评论</summary>
        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCommentCommand(id));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }
    }
}
