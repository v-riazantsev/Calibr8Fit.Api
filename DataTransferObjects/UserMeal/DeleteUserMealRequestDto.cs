using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.UserMeal
{
    public record DeleteUserMealRequestDto
    {
        [Required]
        public required Guid Id { get; init; }
        [Required]
        public required DateTime DeletedAt { get; init; }
    }
}
