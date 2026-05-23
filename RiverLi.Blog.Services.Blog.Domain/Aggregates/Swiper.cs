using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class Swiper : BaseEntity, IAggregateRoot
    {
        public string Image { get; private set; }
        public string? Title { get; private set; }
        public string? Description { get; private set; }
        public string? Url { get; private set; }
        public int Sort { get; private set; } = 0;
        public bool IsActive { get; private set; } = true;

        private Swiper() { }

        public Swiper(string image, string? title, string? description, string? url, int sort, bool isActive)
        {
            Image = image;
            Title = title;
            Description = description;
            Url = url;
            Sort = sort;
            IsActive = isActive;
        }

        public void Update(string image, string? title, string? description, string? url, int sort, bool isActive)
        {
            Image = image;
            Title = title;
            Description = description;
            Url = url;
            Sort = sort;
            IsActive = isActive;
            UpdateModifyTime();
        }
    }
}
