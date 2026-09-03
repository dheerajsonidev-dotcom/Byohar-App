using Byohar.Domain.Common;

namespace Byohar.Domain.Entities.Identity;

public class Permission : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Group { get; set; }
    public string Description { get; set; }
    public string DisplayName {  get; set; }

    public Permission() { }

    public Permission(Guid RolePermissionTenantId, string RolePermissionDescription, string RolePermissionGroup, string RolePermissionDisplayName) : base()
    {
        TenantId = RolePermissionTenantId;
        Description = RolePermissionDescription;
        Group = RolePermissionGroup;
        DisplayName = RolePermissionDisplayName;
    }

    public Permission(Guid id, Guid RolePermissionTenantId, string RolePermissionDescription, string RolePermissionGroup,string RolePermissionDisplayName) : base()
    {
        Id = id;
        TenantId = RolePermissionTenantId;
        Description = RolePermissionDescription;
        Group = RolePermissionGroup;
        DisplayName = RolePermissionDisplayName;
    }
}
