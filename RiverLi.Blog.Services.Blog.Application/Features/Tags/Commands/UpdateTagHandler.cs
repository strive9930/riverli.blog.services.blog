using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Interfaces;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Tags.Commands;

public class UpdateTagHandler : IRequestHandler<UpdateTagCommand, Result>
{
    private readonly IRepository<Tag, Guid> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTagHandler(IRepository<Tag, Guid> repository, ICurrentUser currentUser, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Result.FailResult("未登录用户无法修改标签");

        var tag = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (tag == null)
            return Result.FailResult("标签不存在");

        tag.Update(request.Name, request.Slug);
        await _repository.UpdateAsync(tag, cancellationToken);
        var saved = await _unitOfWork.SaveEntitiesAsync(cancellationToken);
        if (!saved)
            return Result.FailResult("修改失败，请重试");
        return Result.SuccessResult();
    }
}
