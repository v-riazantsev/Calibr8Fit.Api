using Calibr8Fit.Api.DataTransferObjects.User;

namespace Calibr8Fit.Api.DataTransferObjects.Chat
{
    public class ChatMessageDto
    {
        public required Guid Id { get; set; }
        public required Guid ChatId { get; set; }
        public required UserSummaryDto Sender { get; set; }
        public required string Content { get; set; }
        public required DateTime SentAt { get; set; }
        public required bool IsOwnMessage { get; set; }
        public required bool IsReadByUser { get; set; }
        public required bool IsReadByOthers { get; set; }
    }
}