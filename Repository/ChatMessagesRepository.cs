using Calibr8Fit.Api.Data;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Repository.Base;
using Calibr8Fit.Api.Repository.Results;
using Microsoft.EntityFrameworkCore;

// TODO: Make dry?
namespace Calibr8Fit.Api.Repository
{
    public class ChatMessagesRepository(
        ApplicationDbContext dbContext
    ) : RepositoryBase<ChatMessage, Guid>(dbContext), IChatMessagesRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<ChatMessageDetailed?> AddAndReturnDetailedAsync(ChatMessage message)
        {
            var addedMessage = await base.AddAsync(message);

            if (addedMessage is null)
                return null;

            return await _dbSet
                .AsNoTracking()
                .Where(m => m.Id == addedMessage.Id)
                .Select(m => new ChatMessageDetailed
                {
                    Id = m.Id,
                    ChatId = m.ChatId,
                    SenderUserId = m.UserId,
                    SenderUserName = m.User!.UserName!,
                    SenderFirstName = m.User.Profile!.FirstName,
                    SenderLastName = m.User.Profile.LastName,
                    SenderProfilePictureFileName = m.User.Profile.ProfilePictureFileName,

                    Content = m.Content,
                    SentAt = m.SentAt,

                    IsOwnMessage = true, // Since the message was just added by the current user
                    IsReadByRequester = true,
                    IsReadByOthers = false
                })
                .FirstOrDefaultAsync();
        }
        public async Task<List<ChatMessageDetailed>> GetDetailedChatMessagesAsync(Guid chatId, string requesterUserId)
        {
            return await _dbSet
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.SentAt)
                .Include(m => m.User)
                .Select(m => new ChatMessageDetailed
                {
                    Id = m.Id,
                    ChatId = m.ChatId,
                    SenderUserId = m.UserId,
                    SenderUserName = m.User!.UserName!,
                    SenderFirstName = m.User.Profile!.FirstName,
                    SenderLastName = m.User.Profile.LastName,
                    SenderProfilePictureFileName = m.User.Profile.ProfilePictureFileName,
                    Content = m.Content,
                    SentAt = m.SentAt,

                    IsOwnMessage = m.UserId == requesterUserId,
                    IsReadByRequester = m.Reads.Any(r => r.UserId == requesterUserId),
                    IsReadByOthers = m.Reads.Any(r => r.UserId != requesterUserId)
                })
                .ToListAsync();
        }
        public async Task<List<ChatMessageDetailed>> GetDetailedChatMessagesAsync(
            Guid chatId,
            string requesterUserId,
            Guid before,
            int pageSize
            )
        {
            var beforeMessage = await _dbSet.FindAsync(before);

            // If the message with the specified 'before' ID does not exist, throw an exception
            if (beforeMessage is null)
                throw new ArgumentException("The specified 'before' message ID does not exist.", nameof(before));

            // Fetch messages from the chat that were sent before the specified message
            return await _dbSet
                .Where(m => m.ChatId == chatId && m.SentAt < beforeMessage.SentAt)
                .OrderByDescending(m => m.SentAt)
                .Take(pageSize)
                .Include(m => m.User)
                .Select(m => new ChatMessageDetailed
                {
                    Id = m.Id,
                    ChatId = m.ChatId,
                    SenderUserId = m.UserId,
                    SenderUserName = m.User!.UserName!,
                    SenderFirstName = m.User.Profile!.FirstName,
                    SenderLastName = m.User.Profile.LastName,
                    SenderProfilePictureFileName = m.User.Profile.ProfilePictureFileName,
                    Content = m.Content,
                    SentAt = m.SentAt,

                    IsOwnMessage = m.UserId == requesterUserId,
                    IsReadByRequester = m.Reads.Any(r => r.UserId == requesterUserId),
                    IsReadByOthers = m.Reads.Any(r => r.UserId != requesterUserId)
                })
                .ToListAsync();
        }

        public async Task<List<ChatMessageDetailed>> GetDetailedDirectChatMessagesAsync(string requesterUserId, string otherUserId)
        {
            return await _dbContext.Chats
                .Where(c => !c.IsGroupChat &&
                            c.Members.Any(m => m.UserId == requesterUserId) &&
                                c.Members.Any(m => m.UserId == otherUserId))
                    .SelectMany(c => c.Messages)
                    .OrderByDescending(m => m.SentAt)
                    .Include(m => m.User)
                    .Select(m => new ChatMessageDetailed
                    {
                        Id = m.Id,
                        ChatId = m.ChatId,
                        SenderUserId = m.UserId,
                        SenderUserName = m.User!.UserName!,
                        SenderFirstName = m.User.Profile!.FirstName,
                        SenderLastName = m.User.Profile.LastName,
                        SenderProfilePictureFileName = m.User.Profile.ProfilePictureFileName,
                        Content = m.Content,
                        SentAt = m.SentAt,

                        IsOwnMessage = m.UserId == requesterUserId,
                        IsReadByRequester = m.Reads.Any(r => r.UserId == requesterUserId),
                        IsReadByOthers = m.Reads.Any(r => r.UserId != requesterUserId)
                    })
                    .ToListAsync();
        }

        public async Task<List<ChatMessageDetailed>> GetDetailedDirectChatMessagesAsync(
            string requesterUserId,
            string otherUserId,
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
                            c.Members.Any(m => m.UserId == requesterUserId) &&
                                c.Members.Any(m => m.UserId == otherUserId))
                    .SelectMany(c => c.Messages)
                    .Where(m => m.SentAt < beforeMessage.SentAt)
                    .OrderByDescending(m => m.SentAt)
                    .Take(pageSize)
                    .Include(m => m.User)
                    .Select(m => new ChatMessageDetailed
                    {
                        Id = m.Id,
                        ChatId = m.ChatId,
                        SenderUserId = m.UserId,
                        SenderUserName = m.User!.UserName!,
                        SenderFirstName = m.User.Profile!.FirstName,
                        SenderLastName = m.User.Profile.LastName,
                        SenderProfilePictureFileName = m.User.Profile.ProfilePictureFileName,
                        Content = m.Content,
                        SentAt = m.SentAt,
                        IsOwnMessage = m.UserId == requesterUserId,
                        IsReadByRequester = m.Reads.Any(r => r.UserId == requesterUserId),
                        IsReadByOthers = m.Reads.Any(r => r.UserId != requesterUserId)
                    })
                    .ToListAsync();
        }
    }
}