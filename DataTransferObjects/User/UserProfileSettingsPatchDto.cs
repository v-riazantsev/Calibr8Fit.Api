using System.ComponentModel.DataAnnotations;
using Calibr8Fit.Api.Enums;

namespace Calibr8Fit.Api.DataTransferObjects.User
{
    public record UserProfileSettingsPatchDto
    {
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public DateTime? DateOfBirth { get; init; }
        public Gender? Gender { get; init; }
        public float? TargetWeight { get; init; }
        public float? Height { get; init; }
        public UserActivityLevel? ActivityLevel { get; init; }
        public UserClimate? Climate { get; init; }
        public float? ForcedConsumptionTarget { get; init; }
        public float? ForcedBurnTarget { get; init; }
        public float? ForcedHydrationTarget { get; init; }
        [Required]
        public required DateTime ModifiedAt { get; init; }
    }
}