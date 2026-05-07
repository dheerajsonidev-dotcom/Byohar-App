namespace Byohar.Application.Requests.UploadFiles
{
    public class UploadingFileRequest : UploadChunkRequest
    {
        public List<byte[]> ChunksData { get; set; } = new List<byte[]>();
    }
}
