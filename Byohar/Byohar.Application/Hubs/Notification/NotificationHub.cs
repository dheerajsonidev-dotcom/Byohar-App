using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Byohar.Application.Hubs.Notification;

[Authorize]
public class NotificationHub : Hub
{
    private static Dictionary<string, string> userConnections = new Dictionary<string, string>();
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!string.IsNullOrEmpty(userId))
        {
            userConnections[userId] = Context.ConnectionId;
        }
        //await PresenceTracker.ConnectionOpened(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //await PresenceTracker.ConnectionClosed(Context.ConnectionId);
        if (!string.IsNullOrEmpty(userId))
        {
            userConnections.Remove(userId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}