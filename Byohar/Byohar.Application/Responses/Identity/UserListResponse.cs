using Byohar.Application.Responses.Identity;

namespace Byohar.Application.Identity
{
    public class UserListResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureDataUrl { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }

        public string FullName
        {
            get
            {
                string name = string.Empty;
                if (!string.IsNullOrWhiteSpace(FirstName))
                {
                    name += FirstName;
                }
                if (!string.IsNullOrWhiteSpace(LastName))
                {
                    name += " ";
                    name += LastName;
                }
                return name;
            }
        }

        public List<UserTeamResponse> Teams { get; set; }
        public List<UserSiteResponse> Sites { get; set; }
        public List<UserBuildingResponse> Buildings { get; set; }
    }
}
