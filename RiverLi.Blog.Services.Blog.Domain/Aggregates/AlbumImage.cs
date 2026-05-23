using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class AlbumImage : BaseEntity, IAggregateRoot
    {
        public int CateId { get; private set; }
        public string Url { get; private set; }
        public string? Thumbnail { get; private set; }
        public string? Description { get; private set; }
        public int Size { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        private AlbumImage() { }

        public AlbumImage(int cateId, string url, string? thumbnail, string? description, int size, int width, int height)
        {
            CateId = cateId;
            Url = url;
            Thumbnail = thumbnail;
            Description = description;
            Size = size;
            Width = width;
            Height = height;
        }

        public void Update(string url, string? thumbnail, string? description)
        {
            Url = url;
            Thumbnail = thumbnail;
            Description = description;
            UpdateModifyTime();
        }
    }
}
