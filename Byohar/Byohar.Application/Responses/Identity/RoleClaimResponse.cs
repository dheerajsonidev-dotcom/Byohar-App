namespace Byohar.Application.Responses.Identity
{
    public class RolePermissionResponse
    {
        public int Id { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? UserId { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public bool Selected { get; set; }
    }
}