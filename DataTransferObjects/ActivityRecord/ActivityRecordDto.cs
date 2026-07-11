namespace Calibr8Fit.Api.DataTransferObjects.ActivityRecord
{
    public record ActivityRecordDto
    {
        public required Guid Id { get; init; }
        public required Guid ActivityId { get; init; }
        public required int Duration { get; init; } // Duration in seconds
        public required float CaloriesBurned { get; init; }
        public required DateTime Time { get; init; }
        public required DateTime ModifiedAt { get; init; }
        public required bool Deleted { get; init; }
    }
}
