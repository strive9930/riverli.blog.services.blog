using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class Footprint : BaseEntity, IAggregateRoot
    {
        public DateTime Time { get; private set; }
        public string Location { get; private set; }
        public string Content { get; private set; }
        public string Images { get; private set; } = "[]";
        public decimal? Longitude { get; private set; }
        public decimal? Latitude { get; private set; }

        private Footprint() { }

        public Footprint(DateTime time, string location, string content, string images, decimal? longitude, decimal? latitude)
        {
            Time = time;
            Location = location;
            Content = content;
            Images = images;
            Longitude = longitude;
            Latitude = latitude;
        }

        public void Update(DateTime time, string location, string content, string images, decimal? longitude, decimal? latitude)
        {
            Time = time;
            Location = location;
            Content = content;
            Images = images;
            Longitude = longitude;
            Latitude = latitude;
            UpdateModifyTime();
        }
    }
}
