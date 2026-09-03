namespace Byohar.Shared.Constants.User
{
    public static class UserConstants
    {
        public const string DefaultPassword = "1q2w3E*";
    }

    public static class LoginIdentityConstant
    {
        public const int Attempt = 5;
        public const int TimeOut = 30;
    }

    public static class UserThemeConstants
    {
        public const string DefaultThemeLayout = "Vertical";
        public const string DefaultThemeMode = "light";
        public const string DefaultSidebarMode = "dark";
        public const string DefaultIconSize = "lg";
    }
    public static class UserLoginTypeConstants
    {
        public const string Database = "Database";
        public const string Windows = "Windows";
    }

    public static class MessageConstants
    {
        public const string UsernameOrTenantIdIncorrect = "Username or Tenant Id is incorrect.";
        public const string AccountDeleted = "Your account has been deleted. Please contact the admin.";
        public const string AccountDeactivated = "Your account has been deactivated. Please contact the admin or manager.";
        public const string UsernameOrPasswordIncorrect = "Username or password is incorrect.";
        public const string AccountBlocked = "Your account is blocked. Try again after 30 minutes.";
        public const string InvalidClientToken = "Invalid client token.";
        public const string PasswordChangeInitiated = "Your password change request has been processed.";
        public const string ProfileUpdateInitiated = "Your profile update request has been processed.";
        public const string ActiveDirectoryServiceNotFound = "Active Directory Service not found.";
        public const string InvalidToken = "Invalid token.";
        public const string PhoneNumberAlreadyUsed = "Phone number {0} is already used.";
        public const string EmailAlreadyUsed = "Email {0} is already used.";
        public const string PasswordResetInitiated = "Password reset link has been sent.";
        public const string UserNotFound = "User Not Found";
        public const string UserCreated = "User created successfully.";
    }

}