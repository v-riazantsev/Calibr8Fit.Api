using Calibr8Fit.Api.Data;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Calibr8Fit.Api.Repository
{
    public class ChatRepository(
        ApplicationDbContext context
    ) : RepositoryBase<Chat, Guid>(context), IChatRepository
    {
        public async Task<List<Chat>> GetUserChatsAsync(string userId) =>
            await _dbSet.Where(c => c.Members.Any(m => m.UserId == userId)).ToListAsync();

        public async Task<Chat?> GetDirectChatBetweenUsersAsync(string userId1, string userId2) =>
            await _dbSet
                .Where(c => !c.IsGroupChat &&
                            c.Members.Any(m => m.UserId == userId1) &&
                            c.Members.Any(m => m.UserId == userId2))
                .FirstOrDefaultAsync();
    }
}