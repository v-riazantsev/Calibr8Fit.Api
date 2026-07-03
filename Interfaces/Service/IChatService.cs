using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Services.Results;

namespace Calibr8Fit.Api.Interfaces.Service
{
    public interface IChatService
    {
        Task<Chat> CreateDirectChatAsync(string userId, string otherUserId);
        Task<Result<ChatMessageDto>> SendDirectMessageAsync(
            SendDirectMessageRequestDto requestDto,
            User sender,
            bool createChatIfNotExists = true
        );
        Task<Result<IEnumerable<ChatMessageDto>>> GetDirectMessagesAsync(
            string userId,
            string otherUserName,
            Guid? before = null,
            int? pageSize = null
        );
        Task<Result<IEnumerable<ChatMessageDto>>> GetChatMessagesAsync(
            Guid chatId,
            string userId,
            Guid? before = null,
            int? pageSize = null
        );
        Task<Result<IEnumerable<ChatPreviewDto>>> GetUserChatsAsync(string userId);
    }
}
