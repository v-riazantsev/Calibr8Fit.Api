using Calibr8Fit.Api.Enums;

namespace Calibr8Fit.Api.DataTransferObjects.User
{
    public record UserProfileSettingsDto
    {
        public required string UserName { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required DateTime DateOfBirth { get; init; }
        public required Gender Gender { get; init; }
        public required float TargetWeight { get; init; }
        public required float Height { get; init; }
        public required UserActivityLevel ActivityLevel { get; init; }
        public required UserClimate Climate { get; init; }
        public float? ForcedConsumptionTarget { get; init; }
        public float? ForcedBurnTarget { get; init; }
        public float? ForcedHydrationTarget { get; init; }
        public required DateTime ModifiedAt { get; init; }
        public required string? ProfilePictureUrl { get; init; }
    }
}