using Calibr8Fit.Api.Models;

namespace Calibr8Fit.Api.Repository.Results
{
    public class ChatWithDetails
    {
        public required Chat Chat { get; init; }
        public required int MemberCount { get; init; }
        public required ChatMessageDetailed? LastMessage { get; init; }
        public required User? DirectMember { get; init; } // Only applicable for direct chats
        public required DateTime? LastReadByRequesterMessageSentAt { get; init; }
        public required DateTime? LastReadByOtherMembersMessageSentAt { get; init; }
        public required int UnreadMessagesCount { get; init; }
    }
}