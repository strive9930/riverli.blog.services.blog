using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;

namespace RiverLi.Blog.Services.Blog.Application.Features.Records.Commands;

public class CreateRecordHandler : IRequestHandler<CreateRecordCommand, Result<Guid>>
{
    private readonly IRepository<Record, Guid> _repository;

    public CreateRecordHandler(IRepository<Record, Guid> repository) => _repository = repository;

    public async Task<Result<Guid>> Handle(CreateRecordCommand request, CancellationToken cancellationToken)
    {
        var record = new Record(request.Content, request.ImageUrls, request.Location, request.IsPublic);
        await _repository.AddAsync(record, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Result<Guid>.SuccessResult(record.Id);
    }
}

public class UpdateRecordHandler : IRequestHandler<UpdateRecordCommand, Result>
{
    private readonly IRepository<Record, Guid> _repository;

    public UpdateRecordHandler(IRepository<Record, Guid> repository) => _repository = repository;

    public async Task<Result> Handle(UpdateRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (record == null) return Result.FailResult("动态不存在");
        record.Update(request.Content, request.ImageUrls, request.Location, request.IsPublic);
        await _repository.UpdateAsync(record, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Result.SuccessResult();
    }
}

public class DeleteRecordHandler : IRequestHandler<DeleteRecordCommand, Result>
{
    private readonly IRepository<Record, Guid> _repository;

    public DeleteRecordHandler(IRepository<Record, Guid> repository) => _repository = repository;

    public async Task<Result> Handle(DeleteRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (record == null) return Result.FailResult("动态不存在");
        await _repository.DeleteAsync(record, cancellationToken);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return Result.SuccessResult();
    }
}
