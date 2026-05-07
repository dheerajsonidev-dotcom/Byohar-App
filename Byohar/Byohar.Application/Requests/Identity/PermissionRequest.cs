namespace Byohar.Application.Requests.Identity
{
    public class PermissionRequest
    {
        public Guid RoleId { get; set; }
        public IList<RolePermissionRequest> RolePermissions { get; set; }
    }
}