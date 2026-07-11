using System.ComponentModel.DataAnnotations;
using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.Activity
{
    public record UpdateActivityRequestDto : IUpdateRequestDto<Guid>
    {
        [Required]
        public required Guid Id { get; init; }
        [Required]
        public required string MajorHeading { get; init; }
        [Required]
        public required float MetValue { get; init; }
        [Required]
        public required string Description { get; init; }
    }
}