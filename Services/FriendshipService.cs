using Calibr8Fit.Api.DataTransferObjects.Friendship;
using Calibr8Fit.Api.Enums;
using Calibr8Fit.Api.Extensions;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Services.Results;
using Microsoft.EntityFrameworkCore;

namespace Calibr8Fit.Api.Services
{
    // Handles friend requests, friendship management, and push notifications
    public class FriendshipService(
        IFriendshipRepository friendshipRepository,
        IRepositoryBase<FriendRequest, string[]> friendRequestRepository,
        IUserRepository userRepository,
        IPathService pathService,
        IPushService pushService) : IFriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository = friendshipRepository;
        private readonly IRepositoryBase<FriendRequest, string[]> _friendRequestRepository = friendRequestRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPathService _pathService = pathService;
        private readonly IPushService _pushService = pushService;

        public async Task<Result<FriendRequestDto>> SendFriendRequestAsync(string requesterId, string addresseeUsername)
        {
            // Validate users are different and not already friends
            var addresseeId = await _userRepository.GetIdByUsernameAsync(addresseeUsername);
            if (addresseeId is null)
                return Result<FriendRequestDto>.Failure($"User with username '{addresseeUsername}' not found");

            // Validate users are different
            if (requesterId == addresseeId)
                return Result<FriendRequestDto>.Failure("Cannot send friend request to yourself");

            // Check if they are already friends
            if (await _friendshipRepository.AreFriendsAsync(requesterId, addresseeId))
                return Result<FriendRequestDto>.Failure("Users are already friends");

            // Check if friend request already exists
            if (await _friendRequestRepository.KeyExistsAsync(requesterId, addresseeId))
                return Result<FriendRequestDto>.Failure("Friend request already sent");

            // Check if there's a reverse friend request (addressee sent to requester)
            if (await _friendRequestRepository.KeyExistsAsync(addresseeId, requesterId))
                return Result<FriendRequestDto>.Failure("This user has already sent you a friend request");

            // Create new friend request
            var friendRequest = new FriendRequest
            {
                RequesterId = requesterId,
                AddresseeId = addresseeId,
                RequestedAt = DateTime.UtcNow
            };

            await _friendRequestRepository.AddAsync(friendRequest);

            // Reload the created request with navigation properties
            var requestWithNavigationProps = await _friendRequestRepository.QuerySingleAsync(q =>
                q.Include(fr => fr.Requester)
                 .Include(fr => fr.Addressee)
                 .Where(fr => fr.RequesterId == requesterId && fr.AddresseeId == addresseeId));

            if (requestWithNavigationProps is null)
                return Result<FriendRequestDto>.Failure("Failed to retrieve created friend request");

            // Send notification to addressee
            var requester = (await _userRepository.GetAsync(requesterId))!;
            await _pushService.PushNotificationAsync(
                addresseeId,
                "New Friend Request",
                $"You have a new friend request from {requester.Profile!.FirstName} {requester.Profile!.LastName}",
                requester.GetProfilePictureUrl(_pathService)
                );

            return Result<FriendRequestDto>.Success(requestWithNavigationProps.ToFriendRequestDto(_pathService));
        }

        public async Task<Result<FriendshipDto>> AcceptFriendRequestAsync(string addresseeId, string requesterUsername)
        {
            // Get requester ID from username
            var requesterId = await _userRepository.GetIdByUsernameAsync(requesterUsername);
            if (requesterId is null)
                return Result<FriendshipDto>.Failure($"User with username '{requesterUsername}' not found");

            // Get the friend request
            if (!await _friendRequestRepository.KeyExistsAsync(requesterId, addresseeId))
                return Result<FriendshipDto>.Failure("Friend request not found");

            // Create friendship
            var friendship = await _friendshipRepository.AddFriendshipAsync(requesterId, addresseeId);

            // Remove the friend request
            await _friendRequestRepository.DeleteAsync(requesterId, addresseeId);

            // Send notification to requester
            var requester = (await _userRepository.GetByUsernameAsync(requesterUsername))!;
            await _pushService.PushNotificationAsync(
                requesterId,
                "Friend Request Accepted",
                $"Your friend request to {requester.Profile!.FirstName} {requester.Profile!.LastName} has been accepted.",
                requester.GetProfilePictureUrl(_pathService)
            );

            return Result<FriendshipDto>.Success(friendship.ToFriendshipDto(addresseeId, _pathService));
        }

        public async Task<Result> RejectFriendRequestAsync(string addresseeId, string requesterUsername)
        {
            // Get requester ID from username
            var requesterId = await _userRepository.GetIdByUsernameAsync(requesterUsername);
            if (requesterId is null)
                return Result.Failure($"User with username '{requesterUsername}' not found");

            // Get the friend request
            if (!await _friendRequestRepository.KeyExistsAsync(requesterId, addresseeId))
                return Result.Failure("Friend request not found");

            // Remove the friend request
            await _friendRequestRepository.DeleteAsync(requesterId, addresseeId);

            // Send notification to requester
            var requester = (await _userRepository.GetByUsernameAsync(requesterUsername))!;
            await _pushService.PushNotificationAsync(
                requesterId,
                "Friend Request Rejected",
                $"Your friend request to {requester.Profile!.FirstName} {requester.Profile!.LastName} has been rejected.",
                requester.GetProfilePictureUrl(_pathService)
            );

            return Result.Success();
        }

        public async Task<Result> CancelFriendRequestAsync(string requesterId, string addresseeUsername)
        {
            // Get addressee ID from username
            var addresseeId = await _userRepository.GetIdByUsernameAsync(addresseeUsername);
            if (addresseeId is null)
                return Result.Failure($"User with username '{addresseeUsername}' not found");

            // Get the friend request
            if (!await _friendRequestRepository.KeyExistsAsync(requesterId, addresseeId))
                return Result.Failure("Friend request not found");

            // Remove the friend request (only the requester can cancel)
            await _friendRequestRepository.DeleteAsync(requesterId, addresseeId);

            return Result.Success();
        }

        public async Task<IEnumerable<FriendRequestDto>> GetPendingFriendRequestsAsync(string userId)
        {
            // Get all friend requests where the user is the addressee
            return (await _friendRequestRepository.QueryAsync(q => q.Where(fr => fr.AddresseeId == userId)))
                .Select(fr => fr.ToFriendRequestDto(_pathService));
        }

        // Get all friend requests where the user is the requester
        public async Task<IEnumerable<FriendRequestDto>> GetSentFriendRequestsAsync(string userId)
        {
            return (await _friendRequestRepository.QueryAsync(q => q.Where(fr => fr.RequesterId == userId)))
                .Select(fr => fr.ToFriendRequestDto(_pathService));
        }

        // Friendship Management
        public async Task<Result> RemoveFriendshipAsync(string userAUsername, string userBUsername)
        {
            // Get user IDs from usernames
            var userAId = await _userRepository.GetIdByUsernameAsync(userAUsername);
            if (userAId is null)
                return Result.Failure($"User with username '{userAUsername}' not found");

            var userBId = await _userRepository.GetIdByUsernameAsync(userBUsername);
            if (userBId is null)
                return Result.Failure($"User with username '{userBUsername}' not found");

            if (!await _friendshipRepository.RemoveFriendshipAsync(userAId, userBId))
                return Result.Failure("Friendship not found or already removed");

            return Result.Success();
        }

        public async Task<bool> AreFriendsAsync(string userAUsername, string userBUsername)
        {
            // Get user IDs from usernames
            var userAId = await _userRepository.GetIdByUsernameAsync(userAUsername);
            if (userAId is null)
                return false;

            var userBId = await _userRepository.GetIdByUsernameAsync(userBUsername);
            if (userBId is null)
                return false;

            return await _friendshipRepository.AreFriendsAsync(userAId, userBId);
        }

        public async Task<Friendship?> GetFriendshipAsync(string userAUsername, string userBUsername)
        {
            // Get user IDs from usernames
            var userAId = await _userRepository.GetIdByUsernameAsync(userAUsername);
            if (userAId is null)
                return null;

            var userBId = await _userRepository.GetIdByUsernameAsync(userBUsername);
            if (userBId is null)
                return null;

            return await _friendshipRepository.GetFriendshipAsync(userAId, userBId);
        }

        public async Task<IEnumerable<User>> GetAllFriendsAsync(string userId)
        {
            return await _friendshipRepository.GetAllFriendsAsync(userId);
        }

        public async Task<IEnumerable<FriendshipDto>> GetUserFriendshipsAsync(string userId) =>
            (await _friendshipRepository.GetUserFriendshipsAsync(userId))
                .Select(f => f.ToFriendshipDto(userId, _pathService));

        public async Task<IEnumerable<FriendshipDto>> GetUserFriendshipsAsyncByUsername(string username)
        {
            var userId = await _userRepository.GetIdByUsernameAsync(username);
            if (userId is null) return [];

            return await GetUserFriendshipsAsync(userId);
        }

        public async Task<IEnumerable<FriendshipDto>> SearchFriendshipsOfUserAsync(
            string username,
            string query,
            int page = 0,
            int size = 10)
        {
            var userId = await _userRepository.GetIdByUsernameAsync(username);
            if (userId is null) return [];

            var friendships = await _friendshipRepository.SearchFriendshipsOfUserAsync(userId, query, page, size);
            return friendships.Select(f => f.ToFriendshipDto(userId, _pathService));
        }

        public async Task<int> GetFriendsCountAsync(string userId) =>
            await _friendshipRepository.GetFriendsCountAsync(userId);

        public async Task<FriendshipStatus> GetFriendshipStatusAsync(string userId, string targetUsername)
        {
            // Get user IDs from username
            var targetUserId = await _userRepository.GetIdByUsernameAsync(targetUsername);

            if (targetUserId is null || userId == targetUserId)
                return FriendshipStatus.None;

            // Check if they are already friends
            if (await _friendshipRepository.AreFriendsAsync(userId, targetUserId))
                return FriendshipStatus.Friends;

            // Check for pending friend requests
            // Check if current user sent request to target user
            if (await _friendRequestRepository.KeyExistsAsync(userId, targetUserId))
                return FriendshipStatus.PendingSent;

            // Check if target user sent request to current user
            if (await _friendRequestRepository.KeyExistsAsync(targetUserId, userId))
                return FriendshipStatus.PendingReceived;

            // No relationship
            return FriendshipStatus.None;
        }
    }
}