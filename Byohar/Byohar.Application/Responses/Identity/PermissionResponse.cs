namespace Byohar.Application.Responses.Identity
{
    public class PermissionResponse
    {
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public List<RolePermissionResponse> RolePermissions { get; set; }
    }
}