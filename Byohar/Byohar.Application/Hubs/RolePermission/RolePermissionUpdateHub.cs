using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Byohar.Application.Hubs.RolePermission;

public class RolePermissionUpdateHub : Hub
{
    public async Task SendPermissionUpdate(List<string> userIds)
    {
        foreach (var userId in userIds)
        {
            if (userConnections.TryGetValue(userId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("RolePermissionUpdate", userId);
            }
        }
    }

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

}