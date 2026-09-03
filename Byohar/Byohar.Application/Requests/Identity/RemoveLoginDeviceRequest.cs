namespace Byohar.Application.Requests.Identity
{
    public class RemoveLoginDeviceRequest
    {
        public string UserId { get; set; }
        public Guid LoginDeviceId { get; set; }
    }
}