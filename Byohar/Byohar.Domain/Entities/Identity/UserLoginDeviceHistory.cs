using Byohar.Domain.Common;

namespace Byohar.Domain.Entities.Identity;

public class UserLoginDeviceHistory : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string DeviceName { get; set; }
    public string DeviceType { get; set; }
    public string Browser { get; set; }
    public string Address { get; set; }
    public DateTime DateTime { get; set; }

    public ApplicationUser User { get; set; }
}
