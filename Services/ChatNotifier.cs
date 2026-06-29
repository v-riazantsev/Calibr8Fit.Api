using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.Hubs;
using Calibr8Fit.Api.Interfaces.Service;
using Microsoft.AspNetCore.SignalR;

namespace Calibr8Fit.Api.Services
{
    public class ChatNotifier(
        IHubContext<ChatHub> hub
    ) : IChatNotifier
    {
        private readonly IHubContext<ChatHub> _hub = hub;

        public async Task NotifyDirectMessageAsync(string senderUsername, string recipientUsername, ChatMessageDto message)
        {
            // Recipient gets incoming message
            await _hub.Clients.Group(recipientUsername)
                .SendAsync("MessageIncoming", message);

            // Sender gets server-ack
            await _hub.Clients.Group(senderUsername)
                .SendAsync("MessageSent", message);
        }
    }
}