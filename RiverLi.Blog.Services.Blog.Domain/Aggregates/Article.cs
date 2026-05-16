using RiverLi.DDD.Core.Domain.Common;
using RiverLi.DDD.Core.Domain.Events;
using System.Xml.Linq;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class Article : BaseEntity, IAggregateRoot
    {
        public string Title { get; private set; }
        public string Content { get; private set; } // 建议使用 Markdown 或 HTML
        public string Summary { get; private set; }
        public string CoverUrl { get; private set; }
        public int ViewCount { get; private set; }

        // 作者信息 (冗余存储，避免频繁查 IdentityService)
        public string AuthorId { get; private set; }
        public string AuthorName { get; private set; }

        // 导航属性
        private readonly List<Comment> _comments = new();
        public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

        private readonly List<ArticleTag> _tags = new();
        public IReadOnlyCollection<ArticleTag> Tags => _tags.AsReadOnly();

        private Article() { } // EF Core 需要

        public Article(string title, string content, string summary, string coverUrl, string authorId, string authorName)
        {
            Title = title;
            Content = content;
            Summary = summary;
            CoverUrl = coverUrl;
            AuthorId = authorId;
            AuthorName = authorName;
            ViewCount = 0;

            // 领域事件：文章已创建
            // AddDomainEvent(new ArticleCreatedEvent(this));
        }

        public void Update(string title, string content, string summary, string coverUrl)
        {
            Title = title;
            Content = content;
            Summary = summary;
            CoverUrl = coverUrl;

            UpdateModifyTime(); // BaseEntity 方法
        }

        public void AddComment(Guid userId, string userName, string content)
        {
            var comment = new Comment(userId, userName, content);
            _comments.Add(comment);
        }
    }
}