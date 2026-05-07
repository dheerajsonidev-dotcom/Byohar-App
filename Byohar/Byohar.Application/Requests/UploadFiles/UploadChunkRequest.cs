using Microsoft.AspNetCore.Http;

namespace Byohar.Application.Requests.UploadFiles
{
    public class UploadChunkRequest
    {
        public string UploadId { get; set; }
        public string UploadType { get; set; }
        public IFormFile File { get; set; }
        public string ChunkIndex { get; set; }
        public string ChunkMax { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string Path { get; set; }
    }
}