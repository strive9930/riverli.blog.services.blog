using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class WallCate : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string? Icon { get; private set; }
        public string? Color { get; private set; }

        private WallCate() { }

        public WallCate(string name, string? icon, string? color)
        {
            Name = name;
            Icon = icon;
            Color = color;
        }

        public void Update(string name, string? icon, string? color)
        {
            Name = name;
            Icon = icon;
            Color = color;
            UpdateModifyTime();
        }
    }
}
