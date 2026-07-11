using Calibr8Fit.Api.DataTransferObjects.User;

namespace Calibr8Fit.Api.DataTransferObjects.Friendship
{
    public record FriendshipDto
    {
        public required UserSummaryDto Friend { get; init; }
        public required DateTime FriendsSince { get; init; }
    }
}