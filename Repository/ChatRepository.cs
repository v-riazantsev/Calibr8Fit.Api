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
                .Select(c => new ChatWithDetails
                {
                    Chat = c,

                    LastMessagePreview = c.Messages
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => new ChatMessagePreview
                        {
                            UserName = m.User!.UserName!,
                            Content = m.Content,
                            SentAt = m.SentAt,
                            IsOwnMessage = m.UserId == userId,
                            IsRead = m.UserId == userId
                                ? m.Reads.Any(r => r.UserId == userId)
                                : m.Reads.Any(r => r.UserId != userId)
                        })
                        .FirstOrDefault(),

                    UnreadMessagesCount = c.Messages.Count(m =>
                        m.UserId != userId &&
                        !m.Reads.Any(r => r.UserId == userId)),

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
                .OrderByDescending(c => c.LastMessagePreview != null ? c.LastMessagePreview.SentAt : DateTime.MinValue)
                .ToListAsync();


        public async Task<Chat?> GetDirectChatBetweenUsersAsync(string userId1, string userId2) =>
            await _dbSet
                .Where(c => !c.IsGroupChat &&
                            c.Members.Any(m => m.UserId == userId1) &&
                            c.Members.Any(m => m.UserId == userId2))
                .FirstOrDefaultAsync();

    }
}