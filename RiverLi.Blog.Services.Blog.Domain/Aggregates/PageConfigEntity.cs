using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class PageConfigEntity : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Config { get; private set; }

        private PageConfigEntity() { }

        public PageConfigEntity(string name, string config)
        {
            Name = name;
            Config = config;
        }

        public void Update(string name, string config)
        {
            Name = name;
            Config = config;
            UpdateModifyTime();
        }
    }
}
