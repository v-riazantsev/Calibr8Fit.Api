using Calibr8Fit.Api.DataTransferObjects.Chat;

namespace Calibr8Fit.Api.Interfaces.Service
{
    public interface IChatNotifier
    {
        Task NotifyDirectMessageAsync(string senderUsername, string recipientUsername, ChatMessageDto message);
    }
}