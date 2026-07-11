using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.UserFood
{
    public record DeleteUserFoodRequestDto
    {
        [Required]
        public required Guid Id { get; init; }
        [Required]
        public required DateTime DeletedAt { get; init; }
    }
}
