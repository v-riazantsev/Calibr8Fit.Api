using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.DataTransferObjects.Chat.Read;
using Calibr8Fit.Api.Services.Results;

namespace Calibr8Fit.Api.Interfaces.Service
{
    public interface IChatNotifier
    {
        Task<Result> NotifyMessageSentAsync(string senderUsername, ChatMessageDto message);
        Task<Result> NotifyMessageIncomingAsync(string recipientId, string recipientUsername, ChatMessageDto message);
        Task<Result> NotifyMessagesReadAsync(string senderUsername, ChatReadDto readDto);
    }
}