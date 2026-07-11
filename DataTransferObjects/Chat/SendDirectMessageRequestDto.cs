using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.Chat
{
    public record SendDirectMessageRequestDto
    {
        [Required]
        public required string RecipientUsername { get; init; }
        [Required]
        public required string Content { get; init; }
        public DateTime SendedAt { get; init; } = DateTime.UtcNow;
    }
}