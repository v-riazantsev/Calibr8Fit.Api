namespace Calibr8Fit.Api.DataTransferObjects.UserMeal
{
    public record AddUserMealItemDto
    {
        public required Guid FoodId { get; init; }
        public required float Quantity { get; init; }
    }
}