using System.ComponentModel.DataAnnotations;

namespace Byohar.Application.Requests.Identity;

public class VerifyResetPasswordTokenRequest
{
    [Required] public string UserId { get; set; }
    [Required] public string Token { get; set; }
}
