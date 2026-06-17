using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;

namespace RiverLi.Blog.Services.Blog.Application.Common.Behaviors;

/// <summary>
/// MediatR 验证管道：在 Command 到达 Handler 之前自动执行 FluentValidation 校验
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var results = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = results.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Any())
            {
                var errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));
                
                // 根据返回类型构造失败结果
                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var failMethod = typeof(TResponse).GetMethod("FailResult", new[] { typeof(string) });
                    if (failMethod != null)
                        return (TResponse)failMethod.Invoke(null, new object[] { errorMessage })!;
                }
                
                if (typeof(TResponse) == typeof(Result))
                {
                    return (TResponse)(object)Result.FailResult(errorMessage);
                }
            }
        }

        return await next();
    }
}
