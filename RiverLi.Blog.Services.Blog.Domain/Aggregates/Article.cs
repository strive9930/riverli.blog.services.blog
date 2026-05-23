using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class Article : BaseEntity, IAggregateRoot
    {
        public string Title { get; private set; }
        public string Content { get; private set; }
        public string? Description { get; private set; }
        public string? Cover { get; private set; }
        public int ViewCount { get; private set; }
        public string Config { get; private set; } = "{}";

        public string AuthorId { get; private set; }
        public string AuthorName { get; private set; }

        private readonly List<Comment> _comments = new();
        public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

        private readonly List<ArticleCategory> _categories = new();
        public IReadOnlyCollection<ArticleCategory> Categories => _categories.AsReadOnly();

        private readonly List<ArticleTag> _tags = new();
        public IReadOnlyCollection<ArticleTag> Tags => _tags.AsReadOnly();

        private Article() { }

        public Article(string title, string content, string? description, string? cover,
            string config, string authorId, string authorName)
        {
            Title = title;
            Content = content;
            Description = description;
            Cover = cover;
            Config = config;
            AuthorId = authorId;
            AuthorName = authorName;
            ViewCount = 0;
        }

        public void Update(string title, string content, string? description, string? cover, string config)
        {
            Title = title;
            Content = content;
            Description = description;
            Cover = cover;
            Config = config;
            UpdateModifyTime();
        }

        public void IncrementViewCount()
        {
            ViewCount++;
        }

        public void AddCategory(int categoryId)
        {
            if (!_categories.Any(c => c.CategoryId == categoryId))
                _categories.Add(new ArticleCategory(Id, categoryId));
        }

        public void SetCategories(IEnumerable<int> categoryIds)
        {
            _categories.Clear();
            foreach (var cid in categoryIds)
                _categories.Add(new ArticleCategory(Id, cid));
        }

        public void AddTag(string tagName)
        {
            if (!_tags.Any(t => t.TagName == tagName))
                _tags.Add(new ArticleTag(tagName));
        }

        public void SetTags(IEnumerable<string> tagNames)
        {
            _tags.Clear();
            foreach (var name in tagNames)
                _tags.Add(new ArticleTag(name));
        }

        public void AddComment(Guid userId, string userName, string content)
        {
            var comment = new Comment(userId, userName, content);
            _comments.Add(comment);
        }
    }
}
