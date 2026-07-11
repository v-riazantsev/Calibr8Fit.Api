using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.Authentication
{
    public record LoginDto
    {
        [Required]
        public required string UserName { get; init; }
        [Required]
        public required string Password { get; init; }
        [Required]
        public required string DeviceId { get; init; }
    }
}