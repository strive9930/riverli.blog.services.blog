using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class EnvConfig : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        private EnvConfig() { }

        public EnvConfig(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public void UpdateValue(string value)
        {
            Value = value;
            UpdateModifyTime();
        }
    }
}
