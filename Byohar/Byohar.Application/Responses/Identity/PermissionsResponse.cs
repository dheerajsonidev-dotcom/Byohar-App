namespace Byohar.Application.Responses.Identity
{
    public class PermissionsResponse
    {
        public Guid Id { get; set; }
        public string Group { get; set; }
        public List<PermissionDisplay> PermissionDisplay { get; set; }

    }
    public class PermissionDisplay
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
