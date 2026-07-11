using Calibr8Fit.Api.DataTransferObjects.User;

namespace Calibr8Fit.Api.DataTransferObjects.Chat
{
    public record ChatDto
    {
        public required Guid Id { get; init; }
        public required string? Name { get; init; }
        public required bool IsGroupChat { get; init; }
        public required string? AvatarUrl { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required IEnumerable<UserSummaryDto> Members { get; init; }
    }
}