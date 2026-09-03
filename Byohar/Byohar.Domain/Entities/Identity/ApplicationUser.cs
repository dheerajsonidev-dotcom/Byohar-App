using Byohar.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Byohar.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>, IEntity<Guid>
    {
        public Guid TenantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePictureDataUrl { get; set; }
        public string? TimeZone { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<UserPermission> UserPermissions { get; set; }
        public virtual ICollection<UserLoginDeviceHistory> LoginDeviceHistories { get; set; }

        public ApplicationUser() : base()
        {
            UserPermissions = new HashSet<UserPermission>();
        }
    }
}