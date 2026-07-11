using System.ComponentModel.DataAnnotations;
using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.UserActivity
{
    public record UpdateUserActivityRequestDto : IUpdateRequestDto<Guid>
    {
        [Required]
        public required Guid Id { get; init; }
        [Required]
        public required string MajorHeading { get; init; }
        [Required]
        public required float MetValue { get; init; }
        [Required]
        public required string Description { get; init; }
        [Required]
        public required DateTime ModifiedAt { get; init; }
        public required bool Deleted { get; init; } = false; // Default to false if not specified
    }
}