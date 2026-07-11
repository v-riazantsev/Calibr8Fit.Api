namespace Calibr8Fit.Api.DataTransferObjects.WaterIntakeRecord
{
    public record WaterIntakeRecordDto
    {
        public required Guid Id { get; init; }
        public required int AmountInMilliliters { get; init; }
        public required DateTime Time { get; init; }
        public required DateTime ModifiedAt { get; init; }
        public required bool Deleted { get; init; }
    }
}
