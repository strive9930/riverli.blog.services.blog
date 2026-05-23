using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class Oss : BaseEntity, IAggregateRoot
    {
        public string Platform { get; private set; }
        public string AccessKey { get; private set; }
        public string SecretKey { get; private set; }
        public string Bucket { get; private set; }
        public string Domain { get; private set; }
        public string BasePath { get; private set; }
        public bool IsDefault { get; private set; }

        private Oss() { }

        public Oss(string platform, string accessKey, string secretKey, string bucket, string domain, string basePath, bool isDefault)
        {
            Platform = platform;
            AccessKey = accessKey;
            SecretKey = secretKey;
            Bucket = bucket;
            Domain = domain;
            BasePath = basePath;
            IsDefault = isDefault;
        }

        public void Update(string platform, string accessKey, string secretKey, string bucket, string domain, string basePath, bool isDefault)
        {
            Platform = platform;
            AccessKey = accessKey;
            SecretKey = secretKey;
            Bucket = bucket;
            Domain = domain;
            BasePath = basePath;
            IsDefault = isDefault;
            UpdateModifyTime();
        }
    }
}
