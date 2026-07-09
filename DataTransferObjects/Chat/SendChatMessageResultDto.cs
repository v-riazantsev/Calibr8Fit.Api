namespace Calibr8Fit.Api.DataTransferObjects.Chat
{
    public class SendChatMessageResultDto
    {
        public required ChatMessageDto Message { get; init; }

        public required IReadOnlyCollection<string> RecipientUsernames { get; init; }
    }
}