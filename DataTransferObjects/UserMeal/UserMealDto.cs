namespace Calibr8Fit.Api.DataTransferObjects.UserMeal
{
    public record UserMealDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public string? Notes { get; init; }
        public List<UserMealItemDto>? MealItems { get; init; } = [];
        public required DateTime ModifiedAt { get; init; }
        public required bool Deleted { get; init; }
    }
}
