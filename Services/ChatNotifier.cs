using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.DataTransferObjects.Chat.Read;
using Calibr8Fit.Api.Hubs;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Services.Results;
using Microsoft.AspNetCore.SignalR;

namespace Calibr8Fit.Api.Services
{
    // Sends real-time message notifications via SignalR and push notifications
    public class ChatNotifier(
    IHubContext<MessengerHub> hub,
    IPushService pushService
    )
    : IChatNotifier
    {
        private readonly IHubContext<MessengerHub> _hub = hub;
        private readonly IPushService _pushService = pushService;
        private static string UserGroup(string username) => $"user:{username}";


        public async Task<Result> NotifyMessageIncomingAsync(
            string recipientId,
            string recipientUsername,
            ChatMessageDto message)
        {
            // Notify the recipient via push notification and SignalR.
            await _pushService.PushNotificationAsync(
                recipientId,
                $"New Message from {message.Sender.FirstName}",
                message.Content);

            await _hub.Clients
                .Group(UserGroup(recipientUsername))
                .SendAsync("MessageIncoming", message.CopyForRecipient());
            return Result.Success();
        }

        public async Task<Result> NotifyMessageSentAsync(
            string senderUsername,
            ChatMessageDto message)
        {
            await _hub.Clients
                .Group(UserGroup(senderUsername))
                .SendAsync("MessageSent", message);
            return Result.Success();
        }

        public async Task<Result> NotifyMessagesReadAsync(
            string senderUsername,
            ChatReadDto dto)
        {
            await _hub.Clients
                .Group(UserGroup(senderUsername))
                .SendAsync("MessagesRead", dto);
            return Result.Success();
        }
    }
}