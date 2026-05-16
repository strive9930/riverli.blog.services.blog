using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class Comment : BaseEntity // 注意：这里使用 BaseEntity<Guid> 自动生成 ID
    {
        public Guid UserId { get; private set; }
        public string UserName { get; private set; }
        public string Content { get; private set; }

        // 简单点，不做无限嵌套评论，只做单层
        public Guid ArticleId { get; private set; }

        private Comment() { }

        public Comment(Guid userId, string userName, string content)
        {
            UserId = userId;
            UserName = userName;
            Content = content;
        }
    }
}