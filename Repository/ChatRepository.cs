using Calibr8Fit.Api.Data;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Repository.Base;
using Calibr8Fit.Api.Repository.Results;
using Microsoft.EntityFrameworkCore;

namespace Calibr8Fit.Api.Repository
{
    public class ChatRepository(
        ApplicationDbContext context
    ) : RepositoryBase<Chat, Guid>(context), IChatRepository
    {
        public async Task<List<Chat>> GetUserChatsAsync(string userId) =>
            await _dbSet
                .Where(c => c.Members.Any(m => m.UserId == userId))
                .Include(c => c.Members)
                    .ThenInclude(m => m.User)
                        .ThenInclude(u => u!.Profile)
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1)) // Include only the last message
                    .ThenInclude(m => m.User)
                        .ThenInclude(u => u!.Profile)
                .ToListAsync();

        public async Task<List<ChatWithDetails>> GetUserChatsWithDetailsAsync(string userId) =>
            await _dbSet
                .AsNoTracking()
                .Where(c => c.Members.Any(m => m.UserId == userId))
                .Select(c => new
                {
                    Chat = c,

                    RequesterLastReadMessage = c.Members
                        .Where(cm => cm.UserId == userId)
                        .Select(cm => cm.LastReadMessage)
                        .FirstOrDefault(),

                    LastMessage = c.Messages
                        .OrderByDescending(m => m.SentAt)
                        .FirstOrDefault(),

                    LastMessageSentAt = c.Messages.Max(m => (DateTime?)m.SentAt),

                    MemberCount = c.Members.Count(),

                    DirectMember = c.IsGroupChat
                        ? null
                        : c.Members
                            .Where(m => m.UserId != userId)
                            .Select(m => new DirectMemberDetails
                            {
                                UserName = m.User!.UserName,

                                FirstName = m.User.Profile != null
                                    ? m.User.Profile.FirstName
                                    : null,

                                LastName = m.User.Profile != null
                                    ? m.User.Profile.LastName
                                    : null,

                                ProfilePictureFileName = m.User.Profile != null
                                    ? m.User.Profile.ProfilePictureFileName
                                    : null
                            })
                            .FirstOrDefault()
                })
                .Select(x => new
                {
                    ChatWithDetails = new ChatWithDetails
                    {
                        Chat = x.Chat,

                        LastMessagePreview = x.LastMessage == null
                            ? null
                            : new ChatMessagePreview
                            {
                                UserName = x.LastMessage.User!.UserName!,
                                Content = x.LastMessage.Content,
                                SentAt = x.LastMessage.SentAt,
                                IsOwnMessage = x.LastMessage.UserId == userId,

                                IsReadByRequester = x.LastMessage.UserId == userId ||
                                (
                                    x.RequesterLastReadMessage != null &&
                                    x.LastMessage.SentAt <= x.RequesterLastReadMessage.SentAt
                                ),

                                IsReadByOthers =
                                    x.LastMessage.UserId == userId &&
                                    x.Chat.Members.Any(cm =>
                                        cm.UserId != userId &&
                                        cm.LastReadMessage != null &&
                                        cm.LastReadMessage.SentAt >= x.LastMessage.SentAt)
                            },

                        LastReadMessageId = x.Chat.Members
                            .Where(cm => cm.UserId == userId)
                            .Select(cm => cm.LastReadMessageId)
                            .FirstOrDefault(),

                        UnreadMessagesCount = x.Chat.Messages.Count(m =>
                            m.UserId != userId &&
                            (
                                x.RequesterLastReadMessage == null ||
                                m.SentAt > x.RequesterLastReadMessage.SentAt
                            )),

                        MemberCount = x.MemberCount,

                        DirectMember = x.DirectMember
                    },

                    x.LastMessageSentAt
                })
                .OrderByDescending(x => x.LastMessageSentAt ?? DateTime.MinValue)
                .Select(x => x.ChatWithDetails)
                .ToListAsync();

        public async Task<Chat?> GetDirectChatBetweenUsersAsync(string userId1, string userId2) =>
            await _dbSet
                .Where(c => !c.IsGroupChat &&
                            c.Members.Any(m => m.UserId == userId1) &&
                            c.Members.Any(m => m.UserId == userId2))
                .FirstOrDefaultAsync();

    }
}