using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class Config : BaseEntity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string? Notes { get; private set; }

        private Config() { }

        public Config(string name, string value, string? notes)
        {
            Name = name;
            Value = value;
            Notes = notes;
        }

        public void UpdateValue(string value, string? notes)
        {
            Value = value;
            Notes = notes;
            UpdateModifyTime();
        }
    }
}
