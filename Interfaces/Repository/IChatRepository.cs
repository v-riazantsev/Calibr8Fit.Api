using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Models;

namespace Calibr8Fit.Api.Interfaces.Repository
{
    public interface IChatRepository : IRepositoryBase<Chat, Guid>
    {
        Task<List<Chat>> GetUserChatsAsync(string userId);
        Task<Chat?> GetDirectChatBetweenUsersAsync(string userId1, string userId2);
    }
}
