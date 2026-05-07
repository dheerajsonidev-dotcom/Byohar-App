using System;

namespace Byohar.Application.Responses.Identity
{
    public class TokenResponse
    {
        public string Id { get; set; }
        public Guid TenantId { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string UserImageURL { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public int loginDeviceId { get; set; }
        public string calcUserId { get; set; }
        public bool IsEnableTwoFactorAuthentication { get; set; }
        public string TwoFactorAuthenticationSecretKey { get; set; }

        public List<UserPermissionResponse> Permissions { get; set; }
    }
}