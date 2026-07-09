using Calibr8Fit.Api.DataTransferObjects.User;

// TODO: use record for every DTO?
namespace Calibr8Fit.Api.DataTransferObjects.Chat
{
    public record class ChatMessageDto
    {
        public required Guid Id { get; init; }
        public required Guid ChatId { get; init; }
        public required UserSummaryDto Sender { get; init; }
        public required string Content { get; init; }
        public required DateTime SentAt { get; init; }
        public required bool IsOwnMessage { get; init; }

        public ChatMessageDto CopyForRecipient()
        {
            return this with
            {
                IsOwnMessage = false
            };
        }
    }
}