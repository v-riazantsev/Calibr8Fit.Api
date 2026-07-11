using System.ComponentModel.DataAnnotations;

namespace Calibr8Fit.Api.DataTransferObjects.Token
{
    public record TokenDto
    {
        [Required]
        public required string AccessToken { get; init; }
        [Required]
        public required string RefreshToken { get; init; }
    }
}