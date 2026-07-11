using System.ComponentModel.DataAnnotations;
using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.ConsumptionRecord
{
    public record UpdateConsumptionRecordRequestDto : IUpdateRequestDto<Guid>
    {
        [Required]
        public required Guid Id { get; init; }
        public Guid? FoodId { get; init; }
        public Guid? UserMealId { get; init; }
        [Required]
        public required float Quantity { get; init; } // Quantity in grams
        [Required]
        public required DateTime Time { get; init; }
        [Required]
        public required DateTime ModifiedAt { get; init; }
        public required bool Deleted { get; init; } = false; // Default to false if not specified
    }
}
