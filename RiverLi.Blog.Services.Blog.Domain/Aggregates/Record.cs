using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class Record : BaseEntity, IAggregateRoot
    {
        public string Content { get; private set; }
        public string Images { get; private set; } = "[]";

        private Record() { }

        public Record(string content, string images)
        {
            Content = content;
            Images = images;
        }

        public void Update(string content, string images)
        {
            Content = content;
            Images = images;
            UpdateModifyTime();
        }
    }
}
