using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class Category : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string? Icon { get; private set; }
        public Guid? ParentId { get; private set; }

        private Category() { }

        public Category(string name, string? icon, Guid? parentId = null)
        {
            Name = name;
            Icon = icon;
            ParentId = parentId;
        }

        public void Update(string name, string? icon, Guid? parentId)
        {
            Name = name;
            Icon = icon;
            ParentId = parentId;
            UpdateModifyTime();
        }
    }
}
