namespace Byohar.Application.Requests.Identity
{
    public class CreateUpdateUserPermissionRequest
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<UserPermissionRequest> UserPermissionRequests {  get; set; }

    }

    public class UserPermissionRequest
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public Guid ApplicationUserId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
