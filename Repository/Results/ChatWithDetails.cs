using Calibr8Fit.Api.Models;

namespace Calibr8Fit.Api.Repository.Results
{
    public class ChatWithDetails
    {
        public required Chat Chat { get; init; }
        public required int MemberCount { get; init; }
        public required ChatMessagePreview? LastMessagePreview { get; init; }
        public required DirectMemberDetails? DirectMember { get; init; } // Only applicable for direct chats
        public required Guid? LastReadMessageId { get; init; }
        public required int UnreadMessagesCount { get; init; }
    }
}