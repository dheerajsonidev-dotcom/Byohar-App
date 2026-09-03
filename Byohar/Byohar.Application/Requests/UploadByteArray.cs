using System.Text.Json.Serialization;

namespace Byohar.Application.Requests;

public class UploadByteArray
{
    public string FileName { get; set; }
    public string ContentBase64 { get; set; }
    public string ContentType { get; set; }

    [JsonIgnore]
    public byte[] Content
    {
        get => ContentBase64 != null ? Convert.FromBase64String(ContentBase64) : null;
        set => ContentBase64 = value != null ? Convert.ToBase64String(value) : null;
    }
}