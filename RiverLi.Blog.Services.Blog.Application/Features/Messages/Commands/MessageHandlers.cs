using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Messages.Commands;

public class AuditMessageHandler : IRequestHandler<AuditMessageCommand, Result>
{
    private readonly IRepository<Message, Guid> _repository;

    public AuditMessageHandler(IRepository<Message, Guid> repository) => _repository = repository;

    public async Task<Result> Handle(AuditMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (message == null) return Result.FailResult("留言不存在");
        message.Audit(request.Status);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Result.SuccessResult();
    }
}

public class DeleteMessageHandler : IRequestHandler<DeleteMessageCommand, Result>
{
    private readonly IRepository<Message, Guid> _repository;

    public DeleteMessageHandler(IRepository<Message, Guid> repository) => _repository = repository;

    public async Task<Result> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (message == null) return Result.FailResult("留言不存在");
        await _repository.DeleteAsync(message, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Result.SuccessResult();
    }
}
