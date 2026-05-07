namespace Byohar.Application.Interfaces.User;

public interface ICurrentUserService
{
    public string UserId { get; }
    public List<KeyValuePair<string, string>> Claims { get; set; }
    public List<string> UserRoles { get; }
    string OriginUrl { get; set; }
    string Color { get; set; }
    //string GetName();
}