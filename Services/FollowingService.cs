using Calibr8Fit.Api.DataTransferObjects.User;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Services.Results;

// TODO: consider moving querying logic to repository layer
namespace Calibr8Fit.Api.Services
{
    // Manages user follow relationships, followers, and following lists
    public class FollowingService(
        IUserRepository userRepository,
        IUserRepositoryBase<UserFollower, (string, string)> userFollowerRepository,
        IPathService pathService
    ) : IFollowingService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUserRepositoryBase<UserFollower, (string, string)> _userFollowerRepository = userFollowerRepository;
        private readonly IPathService _pathService = pathService;
        public async Task<Result> FollowUserAsync(string followerId, string followeeUsername)
        {
            // Resolve username to ID and validate follow relationship doesn't already exist
            var followeeId = await _userRepository.GetIdByUsernameAsync(followeeUsername);
            if (followeeId is null) return Result.Failure("User to follow not found");
            if (followerId == followeeId) return Result.Failure("Cannot follow yourself");

            var existingFollow = await _userFollowerRepository.GetAsync(followerId, followeeId);
            if (existingFollow is not null) return Result.Failure("Already following this user");

            var userFollower = new UserFollower
            {
                FollowerId = followerId,
                FolloweeId = followeeId
            };

            await _userFollowerRepository.AddAsync(userFollower);
            return Result.Success();
        }

        public async Task<Result> UnfollowUserAsync(string followerId, string followeeUsername)
        {
            var followeeId = await _userRepository.GetIdByUsernameAsync(followeeUsername);
            if (followeeId is null) return Result.Failure("User to unfollow not found");
            if (followerId == followeeId) return Result.Failure("Cannot unfollow yourself");

            var userFollower = await _userFollowerRepository.DeleteAsync(followerId, followeeId);
            if (userFollower is null) return Result.Failure("Not following this user");

            return Result.Success();
        }

        public async Task<Result<List<UserSummaryDto>>> GetFollowersAsync(string userId)
        {
            var followers = await _userFollowerRepository.QueryAsync(f => f.Where(f => f.FolloweeId == userId));

            var followerDtos = followers.Select(f => f.Follower!.ToUserSummaryDto(_pathService)).ToList();

            return Result<List<UserSummaryDto>>.Success(followerDtos);
        }

        public async Task<Result<List<UserSummaryDto>>> SearchFollowersAsync(string userId, string query, int page, int size)
        {
            var followers = await _userFollowerRepository.QueryAsync(f => f
                .Where(f => f.FolloweeId == userId && f.Follower!.UserName!.Contains(query))
                .OrderBy(f => f.Follower!.UserName)
                .Skip(page * size)
                .Take(size));

            var followerDtos = followers.Select(f => f.Follower!.ToUserSummaryDto(_pathService)).ToList();

            return Result<List<UserSummaryDto>>.Success(followerDtos);
        }

        public async Task<int> GetFollowersCountAsync(string userId) =>
            await _userFollowerRepository.CountAsync(f => f.FolloweeId == userId);

        public async Task<Result<List<UserSummaryDto>>> GetFollowingAsync(string userId)
        {
            var following = await _userFollowerRepository.QueryAsync(f => f.Where(f => f.FollowerId == userId));

            var followingDtos = following.Select(f => f.Followee!.ToUserSummaryDto(_pathService)).ToList();

            return Result<List<UserSummaryDto>>.Success(followingDtos);
        }
        public async Task<Result<List<UserSummaryDto>>> SearchFollowingAsync(string userId, string query, int page, int size)
        {
            var following = await _userFollowerRepository.QueryAsync(f => f
                .Where(f => f.FollowerId == userId && f.Followee!.UserName!.Contains(query))
                .OrderBy(f => f.Followee!.UserName)
                .Skip(page * size)
                .Take(size));

            var followingDtos = following.Select(f => f.Followee!.ToUserSummaryDto(_pathService)).ToList();

            return Result<List<UserSummaryDto>>.Success(followingDtos);
        }
        public async Task<int> GetFollowingCountAsync(string userId) =>
            await _userFollowerRepository.CountAsync(f => f.FollowerId == userId);
        public async Task<bool> IsFollowingAsync(string userId, string followeeUsername)
        {
            var followeeId = await _userRepository.GetIdByUsernameAsync(followeeUsername);
            if (followeeId is null) return false;

            return await _userFollowerRepository.KeyExistsAsync(userId, followeeId);
        }
    }
}