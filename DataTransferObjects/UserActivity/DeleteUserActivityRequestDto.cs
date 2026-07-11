using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.UserActivity
{
    public record DeleteUserActivityRequestDto
    {
        [Required]
        public required Guid Id { get; init; }
        [Required]
        public required DateTime DeletedAt { get; init; }
    }
}