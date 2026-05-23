using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class LinkType : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string? Icon { get; private set; }

        private LinkType() { }

        public LinkType(string name, string? icon)
        {
            Name = name;
            Icon = icon;
        }

        public void Update(string name, string? icon)
        {
            Name = name;
            Icon = icon;
            UpdateModifyTime();
        }
    }
}
