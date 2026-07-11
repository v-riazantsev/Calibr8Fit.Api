using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.Activity
{
    public record AddActivityRequestDto
    {
        public Guid Id { get; init; } // Optional, will be generated if not provided
        [Required]
        public required string MajorHeading { get; init; }
        [Required]
        public required float MetValue { get; init; }
        [Required]
        public required string Description { get; init; }
    }
}