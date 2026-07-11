using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.DataTransferObjects.Chat.Read;
using Calibr8Fit.Api.Hubs;
using Calibr8Fit.Api.Interfaces.Service;
using Microsoft.AspNetCore.SignalR;

namespace Calibr8Fit.Api.Services
{
    public class ChatNotifier(
    IHubContext<MessengerHub> hub)
    : IChatNotifier
    {
        private readonly IHubContext<MessengerHub> _hub = hub;
        private static string UserGroup(string username) => $"user:{username}";


        public Task NotifyMessageIncomingAsync(
            string recipientUsername,
            ChatMessageDto message)
        {
            return _hub.Clients
                .Group(UserGroup(recipientUsername))
                .SendAsync("MessageIncoming", message.CopyForRecipient());
        }

        public Task NotifyMessageSentAsync(
            string senderUsername,
            ChatMessageDto message)
        {
            return _hub.Clients
                .Group(UserGroup(senderUsername))
                .SendAsync("MessageSent", message);
        }

        public Task NotifyMessagesReadAsync(
            string senderUsername,
            ChatReadDto dto)
        {
            return _hub.Clients
                .Group(UserGroup(senderUsername))
                .SendAsync("MessagesRead", dto);
        }
    }
}