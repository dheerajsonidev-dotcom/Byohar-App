namespace Byohar.Shared.Constants.Common
{
    public class ApplicationConstants
    {
        public const string NotFound = "Not Found";

        public static class RoleConstants
        {
            public const string AdministratorRole = "Administrator";
            public const string User = "User";
            public const string Supplier = "Supplier";
            public const string Technician = "Technician";
            public const string WorkRequester = "Work Requester";
            public const string AdminDescription = "Administrator role with full permissions";
            public const string SupplierDescription = "Supplier Role with Limited Permission";
            public const string TechnicianDescription = "Technician role with Limited permissions";
            public const string WorkRequesterDescription = "Work Requester role with Limited permissions";
        }

        public static class PermissionConstants
        {
            public const string Name = "Permission";
        }

        public static class SignalR
        {
            public const string ChatHubUrl = "/chatHub";
            public const string NotificationHubUrl = "/notificationHub";
            public const string IdleDetectionHubUrl = "/idleDetectionHub";
            public const string RolePermissionUpdateHubUrl = "/rolePermissionUpdateHub";
            public const string SendRegenerateTokens = "RegenerateTokensAsync";
            public const string ReceiveRegenerateTokens = "RegenerateTokens";
            public const string ReceiveChatNotification = "ReceiveChatNotification";
            public const string SendChatNotification = "ChatNotificationAsync";
            public const string ReceiveMessage = "ReceiveMessage";
            public const string SendMessage = "SendMessageAsync";

            public const string OnConnect = "OnConnectAsync";
            public const string ConnectUser = "ConnectUser";
            public const string OnDisconnect = "OnDisconnectAsync";
            public const string DisconnectUser = "DisconnectUser";
            public const string OnChangeRolePermissions = "OnChangeRolePermissions";
            public const string LogoutUsersByRole = "LogoutUsersByRole";

            public const string PingRequest = "PingRequestAsync";
            public const string PingResponse = "PingResponseAsync";
        }

        public static class FileUploadUrl
        {
            public const string Asset = "Images/Asset/";
            public const string Site = "Images/Site/";
            public const string Stock = "Images/Stock/";
            public const string Buildings = "Images/Buildings/";
            public const string Quote = "Images/Quote/";
            public const string Profile = "Images/ProfilePictures/";
            public const string Supplier = "Images/Supplier/";
            public const string Technician = "Images/Technician/";
            public const string Procedure = "Images/Procedure/";
            public const string Support = "Document/Support/";
            public const string WorkRequest = "Document/WorkRequest/";
        }
    }
}
