using Calibr8Fit.Api.Data;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Repository.Base;
using Calibr8Fit.Api.Repository.Results;
using Microsoft.EntityFrameworkCore;

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

                    IsOwnMessage = true,
                    IsReadByRequester = true,
                    IsReadByOthers = false
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<ChatMessageDetailed>> GetDetailedChatMessagesAsync(
            Guid chatId,
            string requesterUserId
        )
        {
            return await _dbSet
                .AsNoTracking()
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.SentAt)
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

                    IsReadByRequester =
                        m.UserId == requesterUserId ||
                        m.Chat!.Members.Any(cm =>
                            cm.UserId == requesterUserId &&
                            cm.LastReadMessage != null &&
                            cm.LastReadMessage.SentAt >= m.SentAt),

                    IsReadByOthers =
                        m.UserId == requesterUserId &&
                        m.Chat!.Members.Any(cm =>
                            cm.UserId != requesterUserId &&
                            cm.LastReadMessage != null &&
                            cm.LastReadMessage.SentAt >= m.SentAt)
                })
                .ToListAsync();
        }

        public async Task<List<ChatMessageDetailed>> GetDetailedChatMessagesAsync(
            Guid chatId,
            string requesterUserId,
            Guid? before,
            int? pageSize
        )
        {
            var beforeMessage = before.HasValue
                ? await _dbSet
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.Id == before && m.ChatId == chatId)
                : null;

            if (before.HasValue && beforeMessage is null)
                throw new ArgumentException(
                    "The specified 'before' message ID does not exist in this chat.",
                    nameof(before)
                );

            return await _dbSet
                .AsNoTracking()
                .Where(m => m.ChatId == chatId && (beforeMessage == null || m.SentAt < beforeMessage.SentAt))
                .OrderByDescending(m => m.SentAt)
                .Take(pageSize ?? int.MaxValue)
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

                    IsReadByRequester =
                        m.UserId == requesterUserId ||
                        m.Chat!.Members.Any(cm =>
                            cm.UserId == requesterUserId &&
                            cm.LastReadMessage != null &&
                            cm.LastReadMessage.SentAt >= m.SentAt),

                    IsReadByOthers =
                        m.UserId == requesterUserId &&
                        m.Chat!.Members.Any(cm =>
                            cm.UserId != requesterUserId &&
                            cm.LastReadMessage != null &&
                            cm.LastReadMessage.SentAt >= m.SentAt)
                })
                .ToListAsync();
        }

        public async Task<List<ChatMessageDetailed>> GetDetailedDirectChatMessagesAsync(
            string requesterUserId,
            string otherUserId
        )
        {
            return await _dbContext.Chats
                .AsNoTracking()
                .Where(c =>
                    !c.IsGroupChat &&
                    c.Members.Any(m => m.UserId == requesterUserId) &&
                    c.Members.Any(m => m.UserId == otherUserId))
                .SelectMany(c => c.Messages)
                .OrderByDescending(m => m.SentAt)
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

                    IsReadByRequester =
                        m.UserId == requesterUserId ||
                        m.Chat!.Members.Any(cm =>
                            cm.UserId == requesterUserId &&
                            cm.LastReadMessage != null &&
                            cm.LastReadMessage.SentAt >= m.SentAt),

                    IsReadByOthers =
                        m.UserId == requesterUserId &&
                        m.Chat!.Members.Any(cm =>
                            cm.UserId != requesterUserId &&
                            cm.LastReadMessage != null &&
                            cm.LastReadMessage.SentAt >= m.SentAt)
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
            var directChat = await _dbContext.Chats
                .AsNoTracking()
                .Where(c =>
                    !c.IsGroupChat &&
                    c.Members.Any(m => m.UserId == requesterUserId) &&
                    c.Members.Any(m => m.UserId == otherUserId))
                .Select(c => new
                {
                    c.Id
                })
                .FirstOrDefaultAsync();

            if (directChat is null)
                return [];

            var beforeMessage = await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == before && m.ChatId == directChat.Id);

            if (beforeMessage is null)
                throw new ArgumentException(
                    "The specified 'before' message ID does not exist in this direct chat.",
                    nameof(before)
                );

            return await _dbSet
                .AsNoTracking()
                .Where(m =>
                    m.ChatId == directChat.Id &&
                    m.SentAt < beforeMessage.SentAt)
                .OrderByDescending(m => m.SentAt)
                .Take(pageSize)
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

                    IsReadByRequester =
                        m.UserId == requesterUserId ||
                        m.Chat!.Members.Any(cm =>
                            cm.UserId == requesterUserId &&
                            cm.LastReadMessage != null &&
                            cm.LastReadMessage.SentAt >= m.SentAt),

                    IsReadByOthers =
                        m.UserId == requesterUserId &&
                        m.Chat!.Members.Any(cm =>
                            cm.UserId != requesterUserId &&
                            cm.LastReadMessage != null &&
                            cm.LastReadMessage.SentAt >= m.SentAt)
                })
                .ToListAsync();
        }
    }
}