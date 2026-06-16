using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Tags.Commands;

public class DeleteTagHandler : IRequestHandler<DeleteTagCommand, Result>
{
    private readonly IRepository<Tag, Guid> _repository;
    private readonly ICurrentUser _currentUser;

    public DeleteTagHandler(IRepository<Tag, Guid> repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Result.FailResult("未登录用户无法删除标签");

        var tag = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (tag == null)
            return Result.FailResult("标签不存在");

        tag.MarkAsDeleted();
        var saved = await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        if (!saved)
            return Result.FailResult("删除失败，请重试");
        return Result.SuccessResult();
    }
}
