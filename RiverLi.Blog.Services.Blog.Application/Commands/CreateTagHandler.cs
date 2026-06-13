using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public class CreateTagHandler : IRequestHandler<CreateTagCommand, Result<Guid>>
{
    private readonly IRepository<Tag, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public CreateTagHandler(IRepository<Tag, Guid> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Result<Guid>.FailResult("未登录用户无法创建标签");

        var tag = new Tag(request.Name, request.Slug);
        await _repository.AddAsync(tag, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Result<Guid>.SuccessResult(tag.Id);
    }
}
