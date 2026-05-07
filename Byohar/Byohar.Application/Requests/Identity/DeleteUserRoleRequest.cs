namespace Byohar.Application.Requests.Identity
{
    public class DeleteUserRoleRequest
    {
        public Guid UserId { get; set; }
        public List<Guid> Roles { get; set; }
    }
}
