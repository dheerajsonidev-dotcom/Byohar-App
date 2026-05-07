using Microsoft.AspNetCore.SignalR;

namespace Byohar.Application.Hubs.Notification;

public class IdleDetectionHub : Hub
{
    private static List<string> _clientIdleStates = new();

    public override async Task OnConnectedAsync()
    {
        _clientIdleStates.Add(Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        _clientIdleStates.Remove(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task EndCurrentConnection()
    {
        if (_clientIdleStates.Count == 1 && _clientIdleStates.Contains(Context.ConnectionId))
            await Clients.Clients(Context.ConnectionId).SendAsync("ReceiveIdleNotification", true);
        else
            await Clients.Clients(Context.ConnectionId).SendAsync("ReceiveIdleNotification", false);
    }
}