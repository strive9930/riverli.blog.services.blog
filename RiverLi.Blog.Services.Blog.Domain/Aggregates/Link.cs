using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class Link : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Url { get; private set; }
        public string? Logo { get; private set; }
        public string? Description { get; private set; }
        public int TypeId { get; private set; }
        public string Status { get; private set; } = "pending";
        public string? Email { get; private set; }

        private Link() { }

        public Link(string name, string url, string? logo, string? description, int typeId, string? email)
        {
            Name = name;
            Url = url;
            Logo = logo;
            Description = description;
            TypeId = typeId;
            Email = email;
            Status = "pending";
        }

        public void Update(string name, string url, string? logo, string? description, int typeId, string? email)
        {
            Name = name;
            Url = url;
            Logo = logo;
            Description = description;
            TypeId = typeId;
            Email = email;
            UpdateModifyTime();
        }

        public void Approve() => Status = "approved";
        public void Reject() => Status = "rejected";
    }
}
