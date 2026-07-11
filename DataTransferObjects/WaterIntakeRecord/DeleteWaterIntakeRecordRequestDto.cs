using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.WaterIntakeRecord
{
    public record DeleteWaterIntakeRecordRequestDto
    {
        [Required]
        public required Guid Id { get; init; }
        [Required]
        public required DateTime DeletedAt { get; init; }
    }
}
