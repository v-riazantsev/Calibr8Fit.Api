namespace Calibr8Fit.Api.DataTransferObjects.Chat
{
    public class ChatMessagePreviewDto
    {
        public required string UserName { get; set; }
        public required string Content { get; set; }
        public required DateTime SentAt { get; set; }
        public required bool IsOwnMessage { get; set; }
        public required bool IsRead { get; set; }
    }
}