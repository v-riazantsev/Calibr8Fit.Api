using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.Chat
{
    public class SendDirectMessageRequestDto
    {
        [Required]
        public required string RecipientUsername { get; set; }
        [Required]
        public required string Content { get; set; }
        public DateTime SendedAt { get; set; } = DateTime.UtcNow;
    }
}