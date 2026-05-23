using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/file")]
    public class FileController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FileController(IMediator mediator) => _mediator = mediator;

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // File upload logic - placeholder for storage integration
            return Ok(new { message = "文件上传功能需要配置存储服务" });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteFileCommand(id));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpPost("list")]
        [Authorize]
        public async Task<IActionResult> List([FromBody] GetFileListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
