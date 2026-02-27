namespace Calibr8Fit.Api.DataTransferObjects.Chat
{
    public class ChatMessageDto
    {
        public required Guid Id { get; set; }
        public required Guid ChatId { get; set; }
        public required string SenderUsername { get; set; }
        public required string Content { get; set; }
        public required DateTime SendedAt { get; set; }
        public required DateTime? ReadAt { get; set; }
    }
}