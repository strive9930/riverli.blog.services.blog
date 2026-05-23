using MediatR;
using RiverLi.DDD.Core.Application.Common.Models;
using RiverLi.DDD.Core.Domain.Repositories;
using RiverLi.Blog.Services.Blog.Domain.Aggregates;
using RiverLi.Blog.Services.Blog.Application.Common.Dto;
using Microsoft.EntityFrameworkCore;

namespace RiverLi.Blog.Services.Blog.Application.Queries
{
    // Swiper
    public record GetSwiperQuery(Guid Id) : IRequest<Result<SwiperDto>>;
    public record GetSwiperListQuery : IRequest<Result<List<SwiperDto>>>;

    // Footprint
    public record GetFootprintQuery(Guid Id) : IRequest<Result<FootprintDto>>;
    public record GetFootprintListQuery : IRequest<Result<List<FootprintDto>>>;

    // Record
    public record GetRecordQuery(Guid Id) : IRequest<Result<RecordDto>>;
    public record GetRecordListQuery : IRequest<Result<List<RecordDto>>>;

    // Rss
    public record GetRssQuery(Guid Id) : IRequest<Result<RssDto>>;
    public record GetRssListQuery : IRequest<Result<List<RssDto>>>;

    // Config
    public record GetConfigQuery(string Name) : IRequest<Result<ConfigDto>>;
    public record GetConfigListQuery : IRequest<Result<List<ConfigDto>>>;
    public record GetPageConfigQuery(string Name) : IRequest<Result<PageConfigDto>>;
    public record GetOssConfigQuery : IRequest<Result<List<OssDto>>>;

    // File
    public class GetFileListQuery : PagedQuery, IRequest<PagedResult<FileDetailDto>>;
    public record DeleteFileCommand(Guid Id) : IRequest<Result>;

    // Handlers

    public class SwiperQueryHandler :
        IRequestHandler<GetSwiperQuery, Result<SwiperDto>>,
        IRequestHandler<GetSwiperListQuery, Result<List<SwiperDto>>>
    {
        private readonly IRepository<Swiper, Guid> _repo;
        public SwiperQueryHandler(IRepository<Swiper, Guid> repo) => _repo = repo;

        public async Task<Result<SwiperDto>> Handle(GetSwiperQuery r, CancellationToken ct)
        {
            var s = await _repo.GetByIdAsync(r.Id, ct);
            if (s == null) return Result<SwiperDto>.FailResult("轮播图不存在", 404);
            return Result<SwiperDto>.SuccessResult(Map(s));
        }
        public async Task<Result<List<SwiperDto>>> Handle(GetSwiperListQuery r, CancellationToken ct)
        {
            var list = await _repo.AsQueryable().Where(s => s.IsActive).OrderBy(s => s.Sort).ToListAsync(ct);
            return Result<List<SwiperDto>>.SuccessResult(list.Select(Map).ToList());
        }
        private static SwiperDto Map(Swiper s) => new(s.Id, s.Image, s.Title, s.Description, s.Url, s.Sort, s.IsActive, s.CreateTime);
    }

    public class FootprintQueryHandler :
        IRequestHandler<GetFootprintQuery, Result<FootprintDto>>,
        IRequestHandler<GetFootprintListQuery, Result<List<FootprintDto>>>
    {
        private readonly IRepository<Footprint, Guid> _repo;
        public FootprintQueryHandler(IRepository<Footprint, Guid> repo) => _repo = repo;

        public async Task<Result<FootprintDto>> Handle(GetFootprintQuery r, CancellationToken ct)
        {
            var f = await _repo.GetByIdAsync(r.Id, ct);
            if (f == null) return Result<FootprintDto>.FailResult("足迹不存在", 404);
            return Result<FootprintDto>.SuccessResult(Map(f));
        }
        public async Task<Result<List<FootprintDto>>> Handle(GetFootprintListQuery r, CancellationToken ct)
        {
            var list = await _repo.AsQueryable().OrderByDescending(f => f.Time).ToListAsync(ct);
            return Result<List<FootprintDto>>.SuccessResult(list.Select(Map).ToList());
        }
        private static FootprintDto Map(Footprint f) => new(f.Id, f.Time, f.Location, f.Content, f.Images, f.Longitude, f.Latitude, f.CreateTime);
    }

    public class RecordQueryHandler :
        IRequestHandler<GetRecordQuery, Result<RecordDto>>,
        IRequestHandler<GetRecordListQuery, Result<List<RecordDto>>>
    {
        private readonly IRepository<Record, Guid> _repo;
        public RecordQueryHandler(IRepository<Record, Guid> repo) => _repo = repo;

        public async Task<Result<RecordDto>> Handle(GetRecordQuery r, CancellationToken ct)
        {
            var rec = await _repo.GetByIdAsync(r.Id, ct);
            if (rec == null) return Result<RecordDto>.FailResult("备忘录不存在", 404);
            return Result<RecordDto>.SuccessResult(Map(rec));
        }
        public async Task<Result<List<RecordDto>>> Handle(GetRecordListQuery r, CancellationToken ct)
        {
            var list = await _repo.AsQueryable().OrderByDescending(rec => rec.CreateTime).ToListAsync(ct);
            return Result<List<RecordDto>>.SuccessResult(list.Select(Map).ToList());
        }
        private static RecordDto Map(Record r) => new(r.Id, r.Content, r.Images, r.CreateTime);
    }

    public class RssQueryHandler :
        IRequestHandler<GetRssQuery, Result<RssDto>>,
        IRequestHandler<GetRssListQuery, Result<List<RssDto>>>
    {
        private readonly IRepository<Rss, Guid> _repo;
        public RssQueryHandler(IRepository<Rss, Guid> repo) => _repo = repo;

        public async Task<Result<RssDto>> Handle(GetRssQuery r, CancellationToken ct)
        {
            var rss = await _repo.GetByIdAsync(r.Id, ct);
            if (rss == null) return Result<RssDto>.FailResult("RSS源不存在", 404);
            return Result<RssDto>.SuccessResult(Map(rss));
        }
        public async Task<Result<List<RssDto>>> Handle(GetRssListQuery r, CancellationToken ct)
        {
            var list = await _repo.GetAllAsync(ct);
            return Result<List<RssDto>>.SuccessResult(list.Select(Map).ToList());
        }
        private static RssDto Map(Rss r) => new(r.Id, r.Name, r.Url, r.Logo, r.Description, r.LastFetchTime, r.IsActive, r.CreateTime);
    }

    public class ConfigQueryHandler :
        IRequestHandler<GetConfigQuery, Result<ConfigDto>>,
        IRequestHandler<GetConfigListQuery, Result<List<ConfigDto>>>,
        IRequestHandler<GetPageConfigQuery, Result<PageConfigDto>>,
        IRequestHandler<GetOssConfigQuery, Result<List<OssDto>>>
    {
        private readonly IRepository<Config, Guid> _configRepo;
        private readonly IRepository<PageConfigEntity, Guid> _pageConfigRepo;
        private readonly IRepository<Oss, Guid> _ossRepo;

        public ConfigQueryHandler(
            IRepository<Config, Guid> configRepo,
            IRepository<PageConfigEntity, Guid> pageConfigRepo,
            IRepository<Oss, Guid> ossRepo)
        {
            _configRepo = configRepo;
            _pageConfigRepo = pageConfigRepo;
            _ossRepo = ossRepo;
        }

        public async Task<Result<ConfigDto>> Handle(GetConfigQuery r, CancellationToken ct)
        {
            var config = await _configRepo.SingleOrDefaultAsync(c => c.Name == r.Name, ct);
            if (config == null) return Result<ConfigDto>.FailResult("配置不存在", 404);
            return Result<ConfigDto>.SuccessResult(new ConfigDto(config.Id, config.Name, config.Value, config.Notes, config.CreateTime));
        }
        public async Task<Result<List<ConfigDto>>> Handle(GetConfigListQuery r, CancellationToken ct)
        {
            var list = await _configRepo.GetAllAsync(ct);
            return Result<List<ConfigDto>>.SuccessResult(list.Select(c => new ConfigDto(c.Id, c.Name, c.Value, c.Notes, c.CreateTime)).ToList());
        }
        public async Task<Result<PageConfigDto>> Handle(GetPageConfigQuery r, CancellationToken ct)
        {
            var pc = await _pageConfigRepo.SingleOrDefaultAsync(p => p.Name == r.Name, ct);
            if (pc == null) return Result<PageConfigDto>.FailResult("页面配置不存在", 404);
            return Result<PageConfigDto>.SuccessResult(new PageConfigDto(pc.Id, pc.Name, pc.Config, pc.CreateTime));
        }
        public async Task<Result<List<OssDto>>> Handle(GetOssConfigQuery r, CancellationToken ct)
        {
            var list = await _ossRepo.GetAllAsync(ct);
            return Result<List<OssDto>>.SuccessResult(list.Select(o => new OssDto(o.Id, o.Platform, o.AccessKey, o.SecretKey, o.Bucket, o.Domain, o.BasePath, o.IsDefault, o.CreateTime)).ToList());
        }
    }

    public class FileQueryHandler :
        IRequestHandler<GetFileListQuery, PagedResult<FileDetailDto>>,
        IRequestHandler<DeleteFileCommand, Result>
    {
        private readonly IRepository<FileDetail, Guid> _repo;
        public FileQueryHandler(IRepository<FileDetail, Guid> repo) => _repo = repo;

        public async Task<PagedResult<FileDetailDto>> Handle(GetFileListQuery r, CancellationToken ct)
        {
            var query = _repo.AsQueryable().OrderByDescending(f => f.CreateTime);
            var total = await query.CountAsync(ct);
            var items = await query.Skip((r.PageIndex - 1) * r.PageSize).Take(r.PageSize).ToListAsync(ct);
            return PagedResult<FileDetailDto>.SuccessResult(
                items.Select(f => new FileDetailDto(f.Id, f.FileName, f.FilePath, f.FileUrl, f.FileSize, f.FileType, f.Platform, f.CreateTime)).ToList(),
                total, r.PageIndex, r.PageSize);
        }

        public async Task<Result> Handle(DeleteFileCommand r, CancellationToken ct)
        {
            await _repo.DeleteByIdAsync(r.Id, ct);
            await _repo.UnitOfWork.SaveEntitiesAsync(ct);
            return Result.SuccessResult();
        }
    }
}
