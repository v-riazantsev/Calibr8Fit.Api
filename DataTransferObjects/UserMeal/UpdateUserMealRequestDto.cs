using System.ComponentModel.DataAnnotations;
using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.UserMeal
{
    public record UpdateUserMealRequestDto : IUpdateRequestDto<Guid>
    {
        [Required]
        public required Guid Id { get; init; }
        [Required]
        public required string Name { get; init; }
        public string? Notes { get; init; }
        public List<AddUserMealItemDto>? MealItems { get; init; } = [];
        [Required]
        public required DateTime ModifiedAt { get; init; }
        public required bool Deleted { get; init; } = false; // Default to false if not specified
    }
}
