namespace Calibr8Fit.Api.DataTransferObjects.Chat.Read
{
    public class ChatReadDto(Guid chatId, DateTime fromMessageSentAt)
    {
        public Guid ChatId { get; init; } = chatId;
        public DateTime FromMessageSentAt { get; init; } = fromMessageSentAt;
    }
}