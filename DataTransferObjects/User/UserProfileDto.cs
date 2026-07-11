using Calibr8Fit.Api.Enums;

namespace Calibr8Fit.Api.DataTransferObjects.User
{
    public record UserProfileDto
    {
        public required string UserName { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required int FriendsCount { get; init; }
        public required int FollowersCount { get; init; }
        public required int FollowingCount { get; init; }
        public required string? Bio { get; init; }
        public required string? ProfilePictureUrl { get; init; }
        public required FriendshipStatus FriendshipStatus { get; init; }
        public required bool FollowedByMe { get; init; }
    }
}