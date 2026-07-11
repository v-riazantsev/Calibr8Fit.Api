using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.WeightRecord
{
    public record AddWeightRecordRequestDto
    {
        public Guid Id { get; init; } // Optional, will be generated if not provided
        [Required]
        public required float Weight { get; init; } // Weight in kilograms
        [Required]
        public required DateTime Time { get; init; }
        public DateTime ModifiedAt { get; init; } = DateTime.UtcNow; // Default to current time if not specified
        public bool Deleted { get; init; } = false; // Default to false if not specified
    }
}
