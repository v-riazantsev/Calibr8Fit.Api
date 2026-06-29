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
            Guid before,
            int pageSize
            )
        {
            var beforeMessage = await _dbSet.FindAsync(before);

            // If the message with the specified 'before' ID does not exist, throw an exception
            if (beforeMessage is null)
                throw new ArgumentException("The specified 'before' message ID does not exist.", nameof(before));
            
            // Fetch messages from the direct chat between the two users that were sent before the specified message
            return await _dbContext.Chats
                .Where(c => !c.IsGroupChat &&
                            c.Members.Any(m => m.UserId == userId1) &&
                            c.Members.Any(m => m.UserId == userId2))
                .SelectMany(c => c.Messages)
                .Where(m => m.SentAt < beforeMessage.SentAt)
                .OrderByDescending(m => m.SentAt)
                .Take(pageSize)
                .ToListAsync();
        } 
    }
}