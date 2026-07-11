namespace Calibr8Fit.Api.DataTransferObjects.DailyBurnTarget
{
    public record DailyBurnTargetDto
    {
        public required Guid Id { get; init; }
        public required Guid ActivityId { get; init; }
        public required int Duration { get; init; } // Duration in seconds
        public required DateTime ModifiedAt { get; init; }
        public required bool Deleted { get; init; }
    }
}