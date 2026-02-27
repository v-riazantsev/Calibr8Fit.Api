using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Models;

namespace Calibr8Fit.Api.Interfaces.Repository
{
    public interface IChatMessagesRepository : IRepositoryBase<ChatMessage, Guid>
    {
        Task<List<ChatMessage>> GetChatMessagesAsync(Guid chatId);
        Task<List<ChatMessage>> GetDirectChatMessagesAsync(string userId1, string userId2);
        Task<List<ChatMessage>> GetDirectChatMessagesAsync(string userId1, string userId2, int pageIndex, int pageSize);
    }
}
