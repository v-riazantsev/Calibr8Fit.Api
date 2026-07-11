namespace Calibr8Fit.Api.DataTransferObjects.UserActivity
{
    public record UserActivityDto
    {
        public required Guid Id { get; init; }
        public required string MajorHeading { get; init; }
        public required float MetValue { get; init; }
        public required string Description { get; init; }
        public required DateTime ModifiedAt { get; init; }
        public required bool Deleted { get; init; }
    }
}