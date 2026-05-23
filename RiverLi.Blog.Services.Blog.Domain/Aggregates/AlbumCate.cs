using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class AlbumCate : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string? Cover { get; private set; }

        private AlbumCate() { }

        public AlbumCate(string name, string? description, string? cover)
        {
            Name = name;
            Description = description;
            Cover = cover;
        }

        public void Update(string name, string? description, string? cover)
        {
            Name = name;
            Description = description;
            Cover = cover;
            UpdateModifyTime();
        }
    }
}
