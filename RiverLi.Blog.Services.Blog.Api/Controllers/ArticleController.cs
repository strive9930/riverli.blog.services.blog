
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RiverLi.Blog.Services.Blog.Application.Commands;

namespace RiverLi.Blog.Services.Blog.Api.Controllers
{
    [Authorize] // 需要登录
    [ApiController]
    [Route("api/articles")]
    public class ArticleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ArticleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateArticleCommand command)
        {
            // ✅ 调试：检查是否收到 Authorization Header
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            Console.WriteLine($"Authorization Header: {authHeader}");
    
            // ✅ 调试：检查用户是否已认证
            Console.WriteLine($"User.Identity.IsAuthenticated: {User.Identity?.IsAuthenticated}");
            Console.WriteLine($"User.Identity.Name: {User.Identity?.Name}");

            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result.Message);
        }
    }
}
