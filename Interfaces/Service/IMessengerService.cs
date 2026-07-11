using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.DataTransferObjects.Chat.Read;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Services.Results;

namespace Calibr8Fit.Api.Interfaces.Service
{
    public interface IMessengerService
    {
        Task<Result<SendChatMessageResultDto>> SendDirectMessageAsync(
            SendDirectMessageRequestDto requestDto,
            User sender);

        Task<Result<SendChatMessageResultDto>> SendChatMessageAsync(
            SendChatMessageRequestDto requestDto,
            User sender);

        Task<Result<ChatReadResultDto>> ReadMessagesAsync(Guid fromMessageId, User user);
    }
}