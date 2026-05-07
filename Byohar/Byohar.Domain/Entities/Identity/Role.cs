using Byohar.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Byohar.Domain.Entities.Identity;

public class Role : IdentityRole<Guid>, IAuditableEntity<Guid>
{
    public Guid TenantId { get; set; }
    public string Description { get; set; }
    public string CreatedBy { get; set; }    
    public DateTime CreatedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual ICollection<RolePermission> RolePermissions { get; set; }

    public Role() : base()
    {
        RolePermissions = new HashSet<RolePermission>();
    }

    public Role(string roleName, string roleDescription = null) : base(roleName)
    {
        RolePermissions = new HashSet<RolePermission>();
        Description = roleDescription;
    }
}