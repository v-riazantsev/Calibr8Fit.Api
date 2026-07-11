using System.ComponentModel.DataAnnotations;
using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.WeightRecord
{
    public record UpdateWeightRecordRequestDto : IUpdateRequestDto<Guid>
    {
        [Required]
        public required Guid Id { get; init; }
        [Required]
        public required float Weight { get; init; } // Weight in kilograms
        [Required]
        public required DateTime Time { get; init; }
        [Required]
        public required DateTime ModifiedAt { get; init; }
        public required bool Deleted { get; init; } = false; // Default to false if not specified
    }
}
