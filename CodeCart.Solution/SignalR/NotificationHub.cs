using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace CodeCart.API.SignalR;

[Authorize]
public class NotificationHub:Hub
{
    private static readonly ConcurrentDictionary<string, string> UserConnections = new();

    public override Task OnConnectedAsync()
    {
       var email = Context?.User?.FindFirstValue(ClaimTypes.Email);

        if(!string.IsNullOrEmpty(email))
            UserConnections[email] = Context!.ConnectionId;

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var email = Context?.User?.FindFirstValue(ClaimTypes.Email);

        if (!string.IsNullOrEmpty(email))
            UserConnections.TryRemove(email, out _);


        return base.OnDisconnectedAsync(exception);
    }

    public static string? GetConnectiodIdByEmail(string email)
    {
        if (string.IsNullOrEmpty(email)) return null;

        UserConnections.TryGetValue(email, out var connectionId);
        return connectionId;
    }
}
