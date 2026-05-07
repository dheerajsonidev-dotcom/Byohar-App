using Microsoft.AspNetCore.Identity;

namespace Byohar.Domain.Entities.Identity
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public virtual Role Role { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
