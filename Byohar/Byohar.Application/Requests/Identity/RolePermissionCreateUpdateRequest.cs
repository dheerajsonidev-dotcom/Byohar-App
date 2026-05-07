using System.ComponentModel.DataAnnotations;

namespace Byohar.Application.Requests.Identity
{
    public class CreateUpdateRoleRequest
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public List<RolePermissionCreateUpdateRequest>? RolePermissions { get; set; }
    }
    public class RolePermissionCreateUpdateRequest
    {
        public int Id { get; set; }
        public Guid RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }     
    }
}
