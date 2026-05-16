using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    // 多对多关联表实体
    public class ArticleTag : IEntity<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ArticleId { get; set; }
        public string TagName { get; set; } // 直接存 TagName 简化设计，也可存 TagId

        private ArticleTag() { }
        public ArticleTag(string tagName)
        {
            TagName = tagName;
        }
    }
}