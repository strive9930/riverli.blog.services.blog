using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Common.Behaviors;

/// <summary>
/// 乐观锁并发处理管道：捕获 SaveChangesAsync 抛出的 DbUpdateConcurrencyException，
/// 返回友好的并发冲突错误信息
/// </summary>
public class ConcurrencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ConcurrencyBehavior<TRequest, TResponse>> _logger;

    public ConcurrencyBehavior(ILogger<ConcurrencyBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "乐观锁并发冲突: {RequestType}", typeof(TRequest).Name);

            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var failMethod = typeof(TResponse).GetMethod("FailResult", new[] { typeof(string) });
                if (failMethod != null)
                    return (TResponse)failMethod.Invoke(null, new object[] { "数据已被其他用户修改，请刷新后重试" })!;
            }

            if (typeof(TResponse) == typeof(Result))
            {
                return (TResponse)(object)Result.FailResult("数据已被其他用户修改，请刷新后重试");
            }

            throw;
        }
    }
}
