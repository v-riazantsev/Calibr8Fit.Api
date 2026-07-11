using Calibr8Fit.Api.DataTransferObjects.User;

namespace Calibr8Fit.Api.DataTransferObjects.Chat
{
    public record ChatPreviewDto
    {
        public required Guid Id { get; init; }
        public required string DisplayName { get; init; }
        public required bool IsGroupChat { get; init; }
        public required string? AvatarUrl { get; init; }
        public required DateTime CreatedAt { get; init; }
        public required int MemberCount { get; init; }
        public required ChatMessageDto? LastMessage { get; init; }

        public UserSummaryDto? DirectMember { get; init; }

        public required DateTime? LastReadByUserMessageSentAt { get; init; }
        public required DateTime? LastReadByOtherMembersMessageSentAt { get; init; }
        public required int UnreadMessagesCount { get; init; }
    }
}