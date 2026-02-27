using Calibr8Fit.Api.Hubs.Abstract;
using Calibr8Fit.Api.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Calibr8Fit.Api.Hubs
{
    public class ChatHub(
        ICurrentUserService currentUserService
    ) : UserHubBase(currentUserService)
    {
        [Authorize]
        public Task SendMessage(string receiver, string message) =>
            WithUser(async user =>
            {
                // Log message sending
                Console.WriteLine($"[SendMessage] {user.UserName} -> {receiver}: {message}");
                // Send the message only to the recipient
                await Clients.Group(receiver).SendAsync("ReceiveMessage", user.UserName, message);
            });

        [Authorize]
        public override Task OnConnectedAsync() =>
            WithUser(async user =>
        {
            // Add user to a group
            Console.WriteLine($"[OnConnected] User connected: {user.UserName} {Context.ConnectionId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, user.UserName!);
            await base.OnConnectedAsync();
        });

        [Authorize]
        public override Task OnDisconnectedAsync(Exception? exception) =>
            WithUser(async user =>
        {
            // Remove user from a group
            Console.WriteLine($"[OnDisconnected] User disconnected: {user.UserName}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.UserName!);
            await base.OnDisconnectedAsync(exception);
        });
    }
}
