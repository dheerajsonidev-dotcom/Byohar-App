namespace Byohar.Application.Requests.Identity
{
    public class UserActivationRequest
    {
        public List<Guid> UserIds { get; set; }
        public Guid RoleId { get; set; }
        public int ChecklistId { get; set; }
    }
}
