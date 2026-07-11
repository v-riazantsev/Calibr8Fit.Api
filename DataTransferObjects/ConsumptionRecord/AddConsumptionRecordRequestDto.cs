using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.ConsumptionRecord
{
    public record AddConsumptionRecordRequestDto
    {
        public Guid Id { get; init; } // Optional, will be generated if not provided
        public Guid? FoodId { get; init; }
        public Guid? UserMealId { get; init; }
        [Required]
        public required float Quantity { get; init; } // Quantity in grams
        [Required]
        public required DateTime Time { get; init; }
        public DateTime ModifiedAt { get; init; } = DateTime.UtcNow; // Default to current time if not specified
        public bool Deleted { get; init; } = false; // Default to false if not specified
    }
}
