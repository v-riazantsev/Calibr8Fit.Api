using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Services.Results;

namespace Calibr8Fit.Api.Interfaces.Service
{
    public interface IChatService
    {
        Task<Chat> CreateDirectChatAsync(string userId1, string userId2);
        Task<Result<ChatMessageDto>> SendDirectMessageAsync(SendDirectMessageRequestDto requestDto, User sender, bool createChatIfNotExists = true);
        Task<Result<IEnumerable<ChatMessageDto>>> GetDirectMessagesAsync(
            string userName1,
            string userName2,
            int? pageIndex = null,
            int? pageSize = null
        );
        Task<Result<IEnumerable<ChatDto>>> GetDirectChatsAsync(string userId);
    }
}
