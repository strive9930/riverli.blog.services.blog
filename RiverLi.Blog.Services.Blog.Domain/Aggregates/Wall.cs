using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class Wall : BaseEntity, IAggregateRoot
    {
        public string Content { get; private set; }
        public string Email { get; private set; }
        public string Name { get; private set; }
        public string? Url { get; private set; }
        public string? Avatar { get; private set; }
        public int CateId { get; private set; }
        public string Status { get; private set; } = "pending";
        public string? Ip { get; private set; }

        private Wall() { }

        public Wall(string content, string email, string name, string? url, string? avatar, int cateId, string? ip)
        {
            Content = content;
            Email = email;
            Name = name;
            Url = url;
            Avatar = avatar;
            CateId = cateId;
            Ip = ip;
            Status = "pending";
        }

        public void Approve() => Status = "approved";
        public void Reject() => Status = "rejected";
    }
}
