

namespace Byohar.Application.Requests.Identity;

public class UserPagedRequest : PagedRequest
{
    public bool? IsAdmin { get; set; } = false;
    public Guid? BuildingId { get; set; }
    public bool IsForAssignedUsers { get; set; } = false;
    public bool IsFromHistory { get; set; } = false;
}