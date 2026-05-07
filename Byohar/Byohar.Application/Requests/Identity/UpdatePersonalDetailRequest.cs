namespace Byohar.Application.Requests.Identity
{
    public class UpdatePersonalDetailRequest
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }  
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }  
        public string? Description { get; set; }  
        public string? TimeZone { get; set; }  
        
        public string? Email { get; set; }
        public UploadByteArray? ProfilePicture { get; set; } = new();
    }
}
