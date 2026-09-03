using System.ComponentModel.DataAnnotations;

namespace Byohar.Application.Requests.Identity
{
    public class TokenRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }


        public UserDeviceInfo? UserDeviceInfo { get; set; }
    }
}