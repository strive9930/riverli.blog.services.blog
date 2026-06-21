using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Messages.Commands;

public class CreateMessageHandler : IRequestHandler<CreateMessageCommand, Result<Guid>>
{
    private readonly IRepository<Message, Guid> _repository;

    public CreateMessageHandler(IRepository<Message, Guid> repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Nickname) || string.IsNullOrWhiteSpace(request.Content))
            return Result<Guid>.FailResult("昵称和留言内容不能为空");

        var message = new Message(request.Nickname.Trim(), request.Content.Trim(), request.Contact?.Trim());
        await _repository.AddAsync(message, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Result<Guid>.SuccessResult(message.Id);
    }
}
