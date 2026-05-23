using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class ArticleCategory : IEntity<Guid>
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ArticleId { get; set; }
        public int CategoryId { get; set; }

        private ArticleCategory() { }
        public ArticleCategory(Guid articleId, int categoryId)
        {
            ArticleId = articleId;
            CategoryId = categoryId;
        }
    }
}
