using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/album")]
    public class AlbumController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AlbumController(IMediator mediator) => _mediator = mediator;

        // Cate
        [HttpPost("cate")]
        [Authorize]
        public async Task<IActionResult> CreateCate([FromBody] CreateAlbumCateCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("cate/{id}")]
        public async Task<IActionResult> GetCate(Guid id)
        {
            var result = await _mediator.Send(new GetAlbumCateQuery(id));
            return result.Success ? Ok(result) : NotFound(result.Message);
        }

        [HttpPatch("cate/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCate(Guid id, [FromBody] UpdateAlbumCateCommand command)
        {
            var result = await _mediator.Send(command with { Id = id });
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpDelete("cate/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCate(Guid id)
        {
            var result = await _mediator.Send(new DeleteAlbumCateCommand(id));
            return Ok(result);
        }

        [HttpGet("cates")]
        public async Task<IActionResult> CateList()
        {
            var result = await _mediator.Send(new GetAlbumCateListQuery());
            return Ok(result);
        }

        // Image
        [HttpPost("image")]
        [Authorize]
        public async Task<IActionResult> UploadImage([FromBody] CreateAlbumImageCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("image/{id}")]
        public async Task<IActionResult> GetImage(Guid id)
        {
            var result = await _mediator.Send(new GetAlbumImageQuery(id));
            return result.Success ? Ok(result) : NotFound(result.Message);
        }

        [HttpDelete("image/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteImage(Guid id)
        {
            var result = await _mediator.Send(new DeleteAlbumImageCommand(id));
            return Ok(result);
        }

        [HttpPost("images")]
        public async Task<IActionResult> ImageList([FromBody] GetAlbumImageListQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
