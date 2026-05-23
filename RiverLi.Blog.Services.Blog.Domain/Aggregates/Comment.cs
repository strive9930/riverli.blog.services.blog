using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class Comment : BaseEntity, IAggregateRoot
    {
        public Guid? ArticleId { get; private set; }
        public string Content { get; private set; }
        public string Email { get; private set; }
        public string Name { get; private set; }
        public string? Url { get; private set; }
        public string? Avatar { get; private set; }
        public Guid ParentId { get; private set; } = Guid.Empty;
        public string Status { get; private set; } = "pending";
        public string? Ip { get; private set; }
        public string? UserAgent { get; private set; }

        public Guid UserId { get; private set; }
        public string UserName { get; private set; }

        private readonly List<Comment> _replies = new();
        public IReadOnlyCollection<Comment> Replies => _replies.AsReadOnly();

        private Comment() { }

        public Comment(Guid userId, string userName, string content)
        {
            UserId = userId;
            UserName = userName;
            Content = content;
            Email = "";
            Name = userName;
            Status = "pending";
        }

        public Comment(Guid? articleId, string content, string email, string name,
            string? url, string? avatar, Guid parentId, string? ip, string? userAgent)
        {
            ArticleId = articleId;
            Content = content;
            Email = email;
            Name = name;
            Url = url;
            Avatar = avatar;
            ParentId = parentId;
            Ip = ip;
            UserAgent = userAgent;
            Status = "pending";
            UserId = Guid.Empty;
            UserName = name;
        }

        public void Approve() => Status = "approved";
        public void Reject() => Status = "rejected";
    }
}
