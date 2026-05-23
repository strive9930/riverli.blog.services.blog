using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Commands;
using RiverLi.Blog.Services.Blog.Application.Queries;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [ApiController]
    [Route("api/record")]
    public class RecordController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RecordController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateRecordCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new GetRecordQuery(id));
            return result.Success ? Ok(result) : NotFound(result.Message);
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRecordCommand command)
        {
            var result = await _mediator.Send(command with { Id = id });
            return result.Success ? Ok(result) : BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteRecordCommand(id));
            return Ok(result);
        }

        [HttpGet("/api/records")]
        public async Task<IActionResult> List()
        {
            var result = await _mediator.Send(new GetRecordListQuery());
            return Ok(result);
        }
    }
}
