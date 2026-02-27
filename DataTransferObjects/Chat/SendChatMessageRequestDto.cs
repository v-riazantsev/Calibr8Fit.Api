using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.Chat
{
    public class SendChatMessageRequestDto
    {
        public Guid Id { get; set; } // Optional, will be generated if not provided
        [Required]
        public required Guid ChatId { get; set; }
        [Required]
        public string Content { get; set; } = string.Empty;
        public DateTime SendedAt { get; set; } = DateTime.UtcNow;
    }
}