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

        public async Task NotifyDirectMessageAsync(string recipientUsername, ChatMessageDto message)
        {
            // Sender gets server-ack
            await _hub.Clients.Group(message.Sender.UserName)
                .SendAsync("MessageSent", message);

            // Recipient gets incoming message
            await _hub.Clients.Group(recipientUsername)
                .SendAsync("MessageIncoming", message.CopyForRecipient());
        }

        public async Task NotifyChatMessageAsync(IEnumerable<string> recipientUsernames, ChatMessageDto message)
        {
            // Sender gets server-ack
            await _hub.Clients.Group(message.Sender.UserName)
                .SendAsync("MessageSent", message);

            // Recipients get incoming message
            foreach (var recipientUsername in recipientUsernames)
            {
                await _hub.Clients.Group(recipientUsername)
                    .SendAsync("MessageIncoming", message.CopyForRecipient());
            }
        }

    }
}