using Calibr8Fit.Api.Models;

namespace Calibr8Fit.Api.Repository.Results
{
    public class ChatMessageDetailed
    {
        public required Guid Id { get; init; }
        public required Guid ChatId { get; init; }
        public required User Sender { get; init; }
        public required string Content { get; init; }
        public required DateTime SentAt { get; init; }
        public required bool IsOwnMessage { get; init; }
    }
}