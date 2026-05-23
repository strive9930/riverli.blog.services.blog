using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class Rss : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string? Logo { get; private set; }
        public string? Description { get; private set; }
        public DateTime? LastFetchTime { get; private set; }
        public bool IsActive { get; private set; } = true;

        private Rss() { }

        public Rss(string name, string url, string? logo, string? description)
        {
            Name = name;
            Url = url;
            Logo = logo;
            Description = description;
            IsActive = true;
        }

        public void Update(string name, string url, string? logo, string? description, bool isActive)
        {
            Name = name;
            Url = url;
            Logo = logo;
            Description = description;
            IsActive = isActive;
            UpdateModifyTime();
        }

        public void MarkFetched()
        {
            LastFetchTime = DateTime.UtcNow;
        }
    }
}
