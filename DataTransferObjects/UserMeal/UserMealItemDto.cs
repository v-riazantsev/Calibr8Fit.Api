namespace Calibr8Fit.Api.DataTransferObjects.UserMeal
{
    public record UserMealItemDto
    {
        public required Guid FoodId { get; init; }
        public required float Quantity { get; init; } // Amount consumed in grams
    }
}
