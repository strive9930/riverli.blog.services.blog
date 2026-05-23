using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/config")]
    public class ConfigController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ConfigController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            var result = await _mediator.Send(new GetConfigQuery(name));
            return result.Success ? Ok(result) : NotFound(result.Message);
        }

        [HttpPatch("{name}")]
        [Authorize]
        public async Task<IActionResult> Update(string name, [FromBody] UpdateConfigByNameCommand command)
        {
            var result = await _mediator.Send(command with { Name = name });
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("/api/configs")]
        [Authorize]
        public async Task<IActionResult> List()
        {
            var result = await _mediator.Send(new GetConfigListQuery());
            return Ok(result);
        }

        [HttpGet("/api/page/config")]
        public async Task<IActionResult> PageConfig([FromQuery] string name = "home")
        {
            var result = await _mediator.Send(new GetPageConfigQuery(name));
            return result.Success ? Ok(result) : NotFound(result.Message);
        }

        [HttpPatch("json")]
        [Authorize]
        public async Task<IActionResult> UpdateJson([FromBody] UpdateConfigJsonCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("/api/oss")]
        [Authorize]
        public async Task<IActionResult> OssConfig()
        {
            var result = await _mediator.Send(new GetOssConfigQuery());
            return Ok(result);
        }

        [HttpPatch("/api/oss/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateOss(Guid id, [FromBody] UpdateOssCommand command)
        {
            var result = await _mediator.Send(command with { Id = id });
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }
    }
}
