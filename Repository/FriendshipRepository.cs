using Calibr8Fit.Api.Data;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Calibr8Fit.Api.Repository
{
    // Manages friendship records with bidirectional user relationship queries
    public class FriendshipRepository(ApplicationDbContext context) : IFriendshipRepository
    {
        private readonly ApplicationDbContext _context = context;
        private readonly DbSet<Friendship> _dbSet = context.Set<Friendship>();

        private static (string userAId, string userBId) SortUserIds(string userAId, string userBId) =>
            string.Compare(userAId, userBId, StringComparison.Ordinal) <= 0
                ? (userAId, userBId)
                : (userBId, userAId);

        public async Task<Friendship?> GetFriendshipAsync(string userAId, string userBId)
        {
            // Normalize user pair and retrieve friendship record
            var (sortedUserAId, sortedUserBId) = SortUserIds(userAId, userBId);

            return await _dbSet
                .Include(f => f.UserA)
                .Include(f => f.UserB)
                .FirstOrDefaultAsync(f => f.UserAId == sortedUserAId && f.UserBId == sortedUserBId);
        }

        public async Task<bool> AreFriendsAsync(string userAId, string userBId)
        {
            var (sortedUserAId, sortedUserBId) = SortUserIds(userAId, userBId);

            return await _dbSet
                .AnyAsync(f => f.UserAId == sortedUserAId && f.UserBId == sortedUserBId);
        }

        public async Task<IEnumerable<User>> GetAllFriendsAsync(string userId)
        {
            var friendships = await _dbSet
                .Include(f => f.UserA)
                .Include(f => f.UserB)
                .Where(f => f.UserAId == userId || f.UserBId == userId)
                .ToListAsync();

            // Return the other user in each friendship
            return friendships.Select(f => f.UserAId == userId ? f.UserB : f.UserA)
                             .Where(u => u != null)
                             .Cast<User>();
        }

        public async Task<int> GetFriendsCountAsync(string userId) =>
            await _dbSet.CountAsync(f => f.UserAId == userId || f.UserBId == userId);

        public async Task<IEnumerable<Friendship>> GetUserFriendshipsAsync(string userId) =>
            await _dbSet
                .Include(f => f.UserA)
                .Include(f => f.UserB)
                .Where(f => f.UserAId == userId || f.UserBId == userId)
                .ToListAsync();

        public async Task<IEnumerable<Friendship>> SearchFriendshipsOfUserAsync(
            string userId,
            string query,
            int page = 0,
            int size = 10) =>
            await _dbSet
                .Include(f => f.UserA)
                .Include(f => f.UserB)
                .Where(f => (f.UserAId == userId && f.UserB!.UserName != null && f.UserB.UserName.Contains(query)) ||
                            (f.UserBId == userId && f.UserA!.UserName != null && f.UserA.UserName.Contains(query)))
                .OrderBy(f => f.UserAId == userId ? f.UserB!.UserName : f.UserA!.UserName)
                .Skip(page * size)
                .Take(size)
                .ToListAsync();

        public async Task<Friendship> AddFriendshipAsync(string userAId, string userBId)
        {
            var (sortedUserAId, sortedUserBId) = SortUserIds(userAId, userBId);

            // Check if friendship already exists
            var existingFriendship = await _dbSet
                .FirstOrDefaultAsync(f => f.UserAId == sortedUserAId && f.UserBId == sortedUserBId);

            if (existingFriendship is not null) return existingFriendship;

            var friendship = new Friendship
            {
                UserAId = sortedUserAId,
                UserBId = sortedUserBId,
                FriendsSince = DateTime.UtcNow
            };

            _dbSet.Add(friendship);
            await _context.SaveChangesAsync();

            // Load the navigation properties
            await _context.Entry(friendship)
                .Reference(f => f.UserA)
                .LoadAsync();
            await _context.Entry(friendship)
                .Reference(f => f.UserB)
                .LoadAsync();

            return friendship;
        }

        public async Task<bool> RemoveFriendshipAsync(string userAId, string userBId)
        {
            var (sortedUserAId, sortedUserBId) = SortUserIds(userAId, userBId);

            var friendship = await _dbSet
                .FirstOrDefaultAsync(f => f.UserAId == sortedUserAId && f.UserBId == sortedUserBId);

            if (friendship == null)
                return false;

            _dbSet.Remove(friendship);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string userAId, string userBId) =>
            await AreFriendsAsync(userAId, userBId);
    }
}