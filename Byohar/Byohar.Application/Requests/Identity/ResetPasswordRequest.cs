using System.ComponentModel.DataAnnotations;

namespace Byohar.Application.Requests.Identity
{
    public class ResetPasswordRequest
    {
        [Required] public Guid UserId { get; set; }
         public string CurrentPassword { get; set; }
        [Required] public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; }
    }
}