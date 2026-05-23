using RiverLi.DDD.Core.Domain.Common;

namespace RiverLi.Blog.Services.Blog.Domain.Aggregates
{
    public class FileDetail : BaseEntity, IAggregateRoot
    {
        public string FileName { get; private set; }
        public string FilePath { get; private set; }
        public string FileUrl { get; private set; }
        public long FileSize { get; private set; }
        public string FileType { get; private set; }
        public string Platform { get; private set; }

        private FileDetail() { }

        public FileDetail(string fileName, string filePath, string fileUrl, long fileSize, string fileType, string platform)
        {
            FileName = fileName;
            FilePath = filePath;
            FileUrl = fileUrl;
            FileSize = fileSize;
            FileType = fileType;
            Platform = platform;
        }
    }
}
