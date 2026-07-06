using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Repository.Results;

namespace Calibr8Fit.Api.Interfaces.Repository
{
    public interface IChatMessagesRepository : IRepositoryBase<ChatMessage, Guid>
    {
        Task<ChatMessageDetailed?> AddAndReturnDetailedAsync(ChatMessage message);
        Task<List<ChatMessageDetailed>> GetDetailedChatMessagesAsync(Guid chatId, string requesterUserId);
        Task<List<ChatMessageDetailed>> GetDetailedChatMessagesAsync(Guid chatId, string requesterUserId, Guid? before, int? pageSize);
        Task<List<ChatMessageDetailed>> GetDetailedDirectChatMessagesAsync(string requesterUserId, string otherUserId);
        Task<List<ChatMessageDetailed>> GetDetailedDirectChatMessagesAsync(string requesterUserId, string otherUserId, Guid before, int pageSize);
    }
}
