using System;
using System.IO;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Infrastructure.Shared.Controllers;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Queries;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/blog/[controller]")]
    public class MediaController : BaseApiController
    {
        private readonly IMediator _mediator;

        public MediaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>媒体文件分页列表</summary>
        [HttpGet("page")]
        public async Task<IActionResult> GetPage([FromQuery] GetMediaPageQuery query)
        {
            var result = await _mediator.Send(query);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>上传图片文件 (用于文章封面或 Markdown 正文插图)</summary>
        [HttpPost("upload-image")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5 MB
        public async Task<ActionResult<Result<string>>> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(Result<string>.FailResult("未检测到有效的文件"));

            var extension = Path.GetExtension(file.FileName);
            using var stream = file.OpenReadStream();
            var command = new UploadImageCommand(stream, extension, file.ContentType);
            var result = await _mediator.Send(command);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>上传媒体文件</summary>
        [HttpPost]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("请选择要上传的文件");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            var command = new UploadMediaCommand(file.FileName, file.ContentType, ms.ToArray());
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        /// <summary>删除媒体文件</summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteMediaCommand(id));
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }
    }
}
