using Microsoft.AspNetCore.Identity;

namespace Byohar.Domain.Entities.Identity;

public class RolePermission : IdentityRoleClaim<Guid>
{
    public virtual Role Role { get; set; }

}