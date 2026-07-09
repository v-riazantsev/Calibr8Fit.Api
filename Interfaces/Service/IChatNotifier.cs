using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.DataTransferObjects.Chat.Read;

namespace Calibr8Fit.Api.Interfaces.Service
{
    public interface IChatNotifier
    {
        Task NotifyMessageSentAsync(string senderUsername, ChatMessageDto message);
        Task NotifyMessageIncomingAsync(string recipientUsername, ChatMessageDto message);
        Task NotifyMessagesReadAsync(string senderUsername, ChatReadDto readDto);
    }
}