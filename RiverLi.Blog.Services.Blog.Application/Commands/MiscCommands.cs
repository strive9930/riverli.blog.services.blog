using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;

namespace RiverLi.Blog.Services.Blog.Application.Commands
{
    // Swiper
    public record CreateSwiperCommand(string Image, string? Title, string? Description,
        string? Url, int Sort, bool IsActive) : IRequest<Result<Guid>>;
    public record UpdateSwiperCommand(Guid Id, string Image, string? Title, string? Description,
        string? Url, int Sort, bool IsActive) : IRequest<Result>;
    public record DeleteSwiperCommand(Guid Id) : IRequest<Result>;

    // Footprint
    public record CreateFootprintCommand(DateTime Time, string Location, string Content,
        string Images, decimal? Longitude, decimal? Latitude) : IRequest<Result<Guid>>;
    public record UpdateFootprintCommand(Guid Id, DateTime Time, string Location, string Content,
        string Images, decimal? Longitude, decimal? Latitude) : IRequest<Result>;
    public record DeleteFootprintCommand(Guid Id) : IRequest<Result>;

    // Record
    public record CreateRecordCommand(string Content, string Images) : IRequest<Result<Guid>>;
    public record UpdateRecordCommand(Guid Id, string Content, string Images) : IRequest<Result>;
    public record DeleteRecordCommand(Guid Id) : IRequest<Result>;

    // Rss
    public record CreateRssCommand(string Name, string Url, string? Logo, string? Description) : IRequest<Result<Guid>>;
    public record UpdateRssCommand(Guid Id, string Name, string Url, string? Logo, string? Description, bool IsActive) : IRequest<Result>;
    public record DeleteRssCommand(Guid Id) : IRequest<Result>;
    public record FetchRssCommand(Guid Id) : IRequest<Result>;

    // Config
    public record UpdateConfigCommand(Guid Id, string Value, string? Notes) : IRequest<Result>;
    public record UpdateConfigByNameCommand(string Name, string Value) : IRequest<Result>;
    public record UpdateConfigJsonCommand(string Name, string Value) : IRequest<Result>;

    // Oss
    public record UpdateOssCommand(Guid Id, string Platform, string AccessKey, string SecretKey,
        string Bucket, string Domain, string BasePath, bool IsDefault) : IRequest<Result>;

    // Handlers

    public class SwiperCommandHandler :
        IRequestHandler<CreateSwiperCommand, Result<Guid>>,
        IRequestHandler<UpdateSwiperCommand, Result>,
        IRequestHandler<DeleteSwiperCommand, Result>
    {
        private readonly IRepository<Swiper, Guid> _repo;
        public SwiperCommandHandler(IRepository<Swiper, Guid> repo) => _repo = repo;

        public async Task<Result<Guid>> Handle(CreateSwiperCommand r, CancellationToken ct)
        {
            var s = new Swiper(r.Image, r.Title, r.Description, r.Url, r.Sort, r.IsActive);
            await _repo.AddAsync(s, ct);
            await _repo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result<Guid>.SuccessResult(s.Id);
        }
        public async Task<Result> Handle(UpdateSwiperCommand r, CancellationToken ct)
        {
            var s = await _repo.GetByIdAsync(r.Id, ct);
            if (s == null) return Result.FailResult("轮播图不存在", 404);
            s.Update(r.Image, r.Title, r.Description, r.Url, r.Sort, r.IsActive);
            await _repo.UpdateAsync(s, ct);
            await _repo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
        public async Task<Result> Handle(DeleteSwiperCommand r, CancellationToken ct)
        {
            await _repo.DeleteByIdAsync(r.Id, ct);
            await _repo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }

    public class FootprintCommandHandler :
        IRequestHandler<CreateFootprintCommand, Result<Guid>>,
        IRequestHandler<UpdateFootprintCommand, Result>,
        IRequestHandler<DeleteFootprintCommand, Result>
    {
        private readonly IRepository<Footprint, Guid> _repo;
        public FootprintCommandHandler(IRepository<Footprint, Guid> repo) => _repo = repo;

        public async Task<Result<Guid>> Handle(CreateFootprintCommand r, CancellationToken ct)
        {
            var f = new Footprint(r.Time, r.Location, r.Content, r.Images, r.Longitude, r.Latitude);
            await _repo.AddAsync(f, ct);
            await _repo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result<Guid>.SuccessResult(f.Id);
        }
        public async Task<Result> Handle(UpdateFootprintCommand r, CancellationToken ct)
        {
            var f = await _repo.GetByIdAsync(r.Id, ct);
            if (f == null) return Result.FailResult("足迹不存在", 404);
            f.Update(r.Time, r.Location, r.Content, r.Images, r.Longitude, r.Latitude);
            await _repo.UpdateAsync(f, ct);
            await _repo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
        public async Task<Result> Handle(DeleteFootprintCommand r, CancellationToken ct)
        {
            await _repo.DeleteByIdAsync(r.Id, ct);
            await _repo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }

    public class RecordCommandHandler :
        IRequestHandler<CreateRecordCommand, Result<Guid>>,
        IRequestHandler<UpdateRecordCommand, Result>,
        IRequestHandler<DeleteRecordCommand, Result>
    {
        private readonly IRepository<Record, Guid> _repo;
        public RecordCommandHandler(IRepository<Record, Guid> repo) => _repo = repo;

        public async Task<Result<Guid>> Handle(CreateRecordCommand r, CancellationToken ct)
        {
            var rec = new Record(r.Content, r.Images);
            await _repo.AddAsync(rec, ct);
            await _repo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result<Guid>.SuccessResult(rec.Id);
        }
        public async Task<Result> Handle(UpdateRecordCommand r, CancellationToken ct)
        {
            var rec = await _repo.GetByIdAsync(r.Id, ct);
            if (rec == null) return Result.FailResult("备忘录不存在", 404);
            rec.Update(r.Content, r.Images);
            await _repo.UpdateAsync(rec, ct);
            await _repo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
        public async Task<Result> Handle(DeleteRecordCommand r, CancellationToken ct)
        {
            await _repo.DeleteByIdAsync(r.Id, ct);
            await _repo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }

    public class RssCommandHandler :
        IRequestHandler<CreateRssCommand, Result<Guid>>,
        IRequestHandler<UpdateRssCommand, Result>,
        IRequestHandler<DeleteRssCommand, Result>,
        IRequestHandler<FetchRssCommand, Result>
    {
        private readonly IRepository<Rss, Guid> _repo;
        public RssCommandHandler(IRepository<Rss, Guid> repo) => _repo = repo;

        public async Task<Result<Guid>> Handle(CreateRssCommand r, CancellationToken ct)
        {
            var rss = new Rss(r.Name, r.Url, r.Logo, r.Description);
            await _repo.AddAsync(rss, ct);
            await _repo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result<Guid>.SuccessResult(rss.Id);
        }
        public async Task<Result> Handle(UpdateRssCommand r, CancellationToken ct)
        {
            var rss = await _repo.GetByIdAsync(r.Id, ct);
            if (rss == null) return Result.FailResult("RSS源不存在", 404);
            rss.Update(r.Name, r.Url, r.Logo, r.Description, r.IsActive);
            await _repo.UpdateAsync(rss, ct);
            await _repo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
        public async Task<Result> Handle(DeleteRssCommand r, CancellationToken ct)
        {
            await _repo.DeleteByIdAsync(r.Id, ct);
            await _repo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
        public async Task<Result> Handle(FetchRssCommand r, CancellationToken ct)
        {
            var rss = await _repo.GetByIdAsync(r.Id, ct);
            if (rss == null) return Result.FailResult("RSS源不存在", 404);
            rss.MarkFetched();
            await _repo.UpdateAsync(rss, ct);
            await _repo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }

    public class ConfigCommandHandler :
        IRequestHandler<UpdateConfigCommand, Result>,
        IRequestHandler<UpdateConfigByNameCommand, Result>,
        IRequestHandler<UpdateConfigJsonCommand, Result>,
        IRequestHandler<UpdateOssCommand, Result>
    {
        private readonly IRepository<Config, Guid> _configRepo;
        private readonly IRepository<Oss, Guid> _ossRepo;

        public ConfigCommandHandler(IRepository<Config, Guid> configRepo, IRepository<Oss, Guid> ossRepo)
        {
            _configRepo = configRepo;
            _ossRepo = ossRepo;
        }

        public async Task<Result> Handle(UpdateConfigCommand r, CancellationToken ct)
        {
            var config = await _configRepo.GetByIdAsync(r.Id, ct);
            if (config == null) return Result.FailResult("配置不存在", 404);
            config.UpdateValue(r.Value, r.Notes);
            await _configRepo.UpdateAsync(config, ct);
            await _configRepo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(UpdateConfigByNameCommand r, CancellationToken ct)
        {
            var config = await _configRepo.SingleOrDefaultAsync(c => c.Name == r.Name, ct);
            if (config == null) return Result.FailResult("配置不存在", 404);
            config.UpdateValue(r.Value, null);
            await _configRepo.UpdateAsync(config, ct);
            await _configRepo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(UpdateConfigJsonCommand r, CancellationToken ct)
        {
            var config = await _configRepo.SingleOrDefaultAsync(c => c.Name == r.Name, ct);
            if (config == null)
            {
                config = new Config(r.Name, r.Value, null);
                await _configRepo.AddAsync(config, ct);
            }
            else
            {
                config.UpdateValue(r.Value, null);
                await _configRepo.UpdateAsync(config, ct);
            }
            await _configRepo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }

        public async Task<Result> Handle(UpdateOssCommand r, CancellationToken ct)
        {
            var oss = await _ossRepo.GetByIdAsync(r.Id, ct);
            if (oss == null) return Result.FailResult("OSS配置不存在", 404);
            oss.Update(r.Platform, r.AccessKey, r.SecretKey, r.Bucket, r.Domain, r.BasePath, r.IsDefault);
            await _ossRepo.UpdateAsync(oss, ct);
            await _ossRepo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }
}
