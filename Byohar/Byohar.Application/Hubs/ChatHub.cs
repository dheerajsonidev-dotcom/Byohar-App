using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Byohar.Application.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private static Dictionary<string, string> userConnections = new Dictionary<string, string>();

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            userConnections[userId] = Context.ConnectionId;
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            userConnections.Remove(userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    //public async Task SendDirectMessage(DirectMessageResponse message)
    //{
    //    if (userConnections.TryGetValue(message.Receiver?.Id.ToString(), out var connectionId))
    //    {
    //        await Clients.Client(connectionId).SendAsync("ReceiveDirectMessage", message);
    //    }
    //}

    public async Task DeleteDirectMessage(string messageId, string reciverId)
    {
        if (userConnections.TryGetValue(reciverId, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("OnDeleteDirectMessage", messageId);
        }
    }

    // Groups

    public async Task JoinGroup(string groupName)
    {
        string sanitizedGroupName = SanitizeGroupName(groupName);
        await Groups.AddToGroupAsync(Context.ConnectionId, sanitizedGroupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        string sanitizedGroupName = SanitizeGroupName(groupName);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sanitizedGroupName);
    }

    public async Task SendGroupMessage(string groupName, object message)
    {
        string sanitizedGroupName = SanitizeGroupName(groupName);
        await Clients.Group(sanitizedGroupName).SendAsync($"ReceiveGroupMessage_{sanitizedGroupName}", message);
    }

    public async Task DeleteGroupMessage(string groupName, string messageId)
    {
        string sanitizedGroupName = SanitizeGroupName(groupName);
        await Clients.Group(sanitizedGroupName).SendAsync($"OnDeleteGroupMessage_{sanitizedGroupName}", messageId);
    }

    private static string SanitizeGroupName(string groupName)
    {
        return groupName.Replace(" ", ""); // Remove spaces from the group name
    }
}
