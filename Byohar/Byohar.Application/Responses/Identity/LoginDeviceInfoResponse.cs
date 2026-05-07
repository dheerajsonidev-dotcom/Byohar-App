using System;

namespace Byohar.Application.Responses.Identity
{
    public class LoginDeviceInfoResponse
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string Browser { get; set; }
        public string Address { get; set; }
        public DateTime DateTime { get; set; }
    }
}