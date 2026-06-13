using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Commands;

public class UpdateTagHandler : IRequestHandler<UpdateTagCommand, Result>
{
    private readonly IRepository<Tag, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public UpdateTagHandler(IRepository<Tag, Guid> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Result.FailResult("未登录用户无法修改标签");

        var tag = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (tag == null)
            return Result.FailResult("标签不存在");

        tag.Update(request.Name, request.Slug);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Result.SuccessResult();
    }
}
