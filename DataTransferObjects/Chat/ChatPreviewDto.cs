using Calibr8Fit.Api.DataTransferObjects.User;

namespace Calibr8Fit.Api.DataTransferObjects.Chat
{
    public class ChatPreviewDto
    {
        public required Guid Id { get; set; }
        public required string DisplayName { get; set; }
        public required bool IsGroupChat { get; set; }
        public required string? AvatarUrl { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required int MemberCount { get; set; }
        public required ChatMessageDto? LastMessage { get; set; }

        public UserSummaryDto? DirectMember { get; set; }
        public required IEnumerable<string> TypingUsernames { get; set; }

        public required DateTime? LastReadByUserMessageSentAt { get; set; }
        public required DateTime? LastReadByOtherMembersMessageSentAt { get; set; }
        public required int UnreadMessagesCount { get; set; }
    }
}