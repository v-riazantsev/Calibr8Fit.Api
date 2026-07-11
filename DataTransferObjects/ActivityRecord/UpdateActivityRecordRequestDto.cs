using System.ComponentModel.DataAnnotations;
using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.ActivityRecord
{
    public record UpdateActivityRecordRequestDto : IUpdateRequestDto<Guid>
    {
        [Required]
        public required Guid Id { get; init; }
        [Required]
        public required Guid ActivityId { get; init; }
        [Required]
        public required int Duration { get; init; } // Duration in seconds
        [Required]
        public required float CaloriesBurned { get; init; }
        [Required]
        public required DateTime Time { get; init; }
        [Required]
        public required DateTime ModifiedAt { get; init; }
        public bool Deleted { get; init; } = false; // Default to false if not specified
    }
}
