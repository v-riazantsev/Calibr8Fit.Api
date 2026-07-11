using Calibr8Fit.Api.DataTransferObjects.User;

namespace Calibr8Fit.Api.DataTransferObjects.Friendship
{
    public record FriendRequestDto
    {
        public required UserSummaryDto Requester { get; init; }
        public required UserSummaryDto Receiver { get; init; }
        public required DateTime RequestedAt { get; init; }
    }
}