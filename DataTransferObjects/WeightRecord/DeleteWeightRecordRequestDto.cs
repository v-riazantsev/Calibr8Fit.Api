using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.WeightRecord
{
    public record DeleteWeightRecordRequestDto
    {
        [Required]
        public required Guid Id { get; init; }
        [Required]
        public required DateTime DeletedAt { get; init; }
    }
}
