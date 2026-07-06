using Calibr8Fit.Api.DataTransferObjects.Chat;

namespace Calibr8Fit.Api.Interfaces.Service
{
    public interface IChatNotifier
    {
        Task NotifyDirectMessageAsync(string recipientUsername, ChatMessageDto message);
        Task NotifyChatMessageAsync(IEnumerable<string> recipientUsernames, ChatMessageDto message);
        Task NotifyChatMessageAsync(string recipientUsername, ChatMessageDto message) =>
            NotifyChatMessageAsync(recipientUsername, message);
    }
}