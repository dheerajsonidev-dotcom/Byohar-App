using Byohar.Application.Interfaces.User;
using System.Security.Claims;
using Byohar.Application.Interfaces.User;

namespace Byohar.Api.Services.User
{
    public class CurrentUserService : ICurrentUserService
    {
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var claimsPrinicipal = httpContextAccessor.HttpContext?.User;
            Claims = claimsPrinicipal?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
            UserId = claimsPrinicipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            UserRoles = claimsPrinicipal?.FindAll(ClaimTypes.Role)?.Select(x => x.Value)?.ToList();
            OriginUrl = httpContextAccessor.HttpContext?.Request.Headers["origin"];
            Color = "Blue";
        }

        public string Color { get;  }
        public string UserId { get; }
        public List<KeyValuePair<string, string>> Claims { get; set; }
        public List<string> UserRoles { get; }
        public string OriginUrl { get; set; }
        string ICurrentUserService.Color { get => Color; set => throw new NotImplementedException(); }
    }
}