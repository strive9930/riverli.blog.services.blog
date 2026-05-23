using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class Tag : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string? Color { get; private set; }

        private Tag() { }

        public Tag(string name, string? color)
        {
            Name = name;
            Color = color;
        }

        public void Update(string name, string? color)
        {
            Name = name;
            Color = color;
            UpdateModifyTime();
        }
    }
}
