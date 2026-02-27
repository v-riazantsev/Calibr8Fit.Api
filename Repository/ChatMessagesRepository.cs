using Calibr8Fit.Api.Data;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Calibr8Fit.Api.Repository
{
    public class ChatMessagesRepository(
        ApplicationDbContext dbContext
    ) : RepositoryBase<ChatMessage, Guid>(dbContext), IChatMessagesRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<List<ChatMessage>> GetChatMessagesAsync(Guid chatId) =>
            await _dbSet.Where(m => m.ChatId == chatId).ToListAsync();

        public async Task<List<ChatMessage>> GetDirectChatMessagesAsync(
            string userId1,
            string userId2
            ) => await _dbContext.Chats
                    .Where(c => !c.IsGroupChat &&
                                c.Members.Any(m => m.UserId == userId1) &&
                                c.Members.Any(m => m.UserId == userId2))
                    .SelectMany(c => c.Messages)
                    .ToListAsync();

        public async Task<List<ChatMessage>> GetDirectChatMessagesAsync(
            string userId1,
            string userId2,
            int pageIndex,
            int pageSize
            ) => await _dbContext.Chats
                    .Where(c => !c.IsGroupChat &&
                                c.Members.Any(m => m.UserId == userId1) &&
                                c.Members.Any(m => m.UserId == userId2))
                    .SelectMany(c => c.Messages)
                    .OrderByDescending(m => m.SentAt)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
    }
}