using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.Chat
{
    public record SendChatMessageRequestDto
    {
        public Guid Id { get; init; } // Optional, will be generated if not provided
        [Required]
        public required Guid ChatId { get; init; }
        [Required]
        public required string Content { get; init; } = string.Empty;
        public DateTime SentAt { get; init; } = DateTime.UtcNow;
    }
}