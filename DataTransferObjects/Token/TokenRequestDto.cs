using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.Token
{
    public record TokenRequestDto
    {
        [Required]
        public required string OldAccessToken { get; init; }
        [Required]
        public required string RefreshToken { get; init; }
        [Required]
        public required string DeviceId { get; init; }
    }
}