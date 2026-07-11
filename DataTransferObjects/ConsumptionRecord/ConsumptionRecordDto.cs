namespace Calibr8Fit.Api.DataTransferObjects.ConsumptionRecord
{
    public record ConsumptionRecordDto
    {
        public required Guid Id { get; init; }
        public Guid? FoodId { get; init; }
        public Guid? UserMealId { get; init; }
        public required float Quantity { get; init; } // Quantity in grams
        public required DateTime Time { get; init; }
        public required DateTime ModifiedAt { get; init; }
        public required bool Deleted { get; init; }
    }
}
