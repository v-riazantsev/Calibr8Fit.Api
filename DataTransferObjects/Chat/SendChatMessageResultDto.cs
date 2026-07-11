namespace Calibr8Fit.Api.DataTransferObjects.Chat
{
    public record SendChatMessageResultDto
    {
        public required ChatMessageDto Message { get; init; }

        public required IReadOnlyCollection<string> RecipientUsernames { get; init; }
    }
}