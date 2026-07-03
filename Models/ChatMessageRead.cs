namespace Calibr8Fit.Api.Models
{
    public class ChatMessageRead
    {
        public required Guid ChatMessageId { get; set; }
        public required ChatMessage ChatMessage { get; set; }

        public required string UserId { get; set; }
        public virtual User? User { get; set; }

        public required DateTime ReadAt { get; set; }
    }
}