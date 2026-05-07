
using Byohar.Shared.Constants.User;
using System.ComponentModel.DataAnnotations;

namespace Byohar.Application.Requests.Identity
{
    public class RegisterRequest
    {
        public Guid? Id { get; set; }
        public Guid? TenantId { get; set; }
        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }
       
        [EmailAddress]
        public string? Email { get; set; }
        [EmailAddress]
        public string? Repeatmail { get; set; }
        public virtual bool EmailConfirmed { get; set; } = false;
        public bool IsActive { get; set; } = false;
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string UserLoginType { get; set; } = UserLoginTypeConstants.Database;
        public List<string> Roles { get; set; }
        public UploadByteArray? ProfilePicture { get; set; } = new();
        public string? Origin { get; set; }

        
    }
}