using Microsoft.AspNetCore.Http;

namespace Byohar.Application.Requests.UploadFiles;

public class UploadRequest
{
    public string Path { get; set; }
    public byte[] FileBytes { get; set; }
    public string FileName { get; set; }
}