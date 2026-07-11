using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.ActivityRecord
{
    public record AddActivityRecordRequestDto
    {
        public Guid Id { get; init; } // Optional, will be generated if not provided
        [Required]
        public required Guid ActivityId { get; init; }
        [Required]
        public required int Duration { get; init; } // Duration in seconds
        [Required]
        public required float CaloriesBurned { get; init; }
        [Required]
        public required DateTime Time { get; init; }
        public DateTime ModifiedAt { get; init; } = DateTime.UtcNow; // Default to current time if not specified
        public bool Deleted { get; init; } = false; // Default to false if not specified
    }
}
