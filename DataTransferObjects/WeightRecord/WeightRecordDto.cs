namespace Calibr8Fit.Api.DataTransferObjects.WeightRecord
{
    public record WeightRecordDto
    {
        public required Guid Id { get; init; }
        public required float Weight { get; init; } // Weight in kilograms
        public required DateTime Time { get; init; }
        public required DateTime ModifiedAt { get; init; }
        public required bool Deleted { get; init; }
    }
}
