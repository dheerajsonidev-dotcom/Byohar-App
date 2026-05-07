namespace Byohar.Application.Responses.Identity
{
    public class ExportUserCSV
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? CreatedOn { get; set; }

    }
}
