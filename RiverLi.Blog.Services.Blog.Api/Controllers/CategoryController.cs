using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Infrastructure.Shared.Controllers;
using RiverLi.Blog.Services.Blog.Application.Features.Categories.Commands;
using RiverLi.Blog.Services.Blog.Application.Features.Categories.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/blog/[controller]")]
    public class CategoryController: BaseApiController
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>公开：获取分类树形结构</summary>
        [AllowAnonymous]
        [HttpGet("tree")]
        public async Task<IActionResult> GetTree()
        {
            var result = await _mediator.Send(new GetCategoryTreeQuery());
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>公开：获取分类扁平选项 (供下拉框使用)</summary>
        [AllowAnonymous]
        [HttpGet("options")]
        public async Task<IActionResult> GetOptions()
        {
            var result = await _mediator.Send(new GetCategoryOptionsQuery());
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>新增分类（需登录）</summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>修改分类信息（需登录）</summary>
        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
        {
            if (id != command.Id)
                return BadRequest("路由 ID 与请求体 ID 不一致");

            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>删除分类 - 软删除（需登录）</summary>
        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }
    }
}
