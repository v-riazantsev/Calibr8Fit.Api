namespace Calibr8Fit.Api.DataTransferObjects.Activity
{
    public record ActivityDto
    {
        public required Guid Id { get; init; }
        public required string MajorHeading { get; init; }
        public required float MetValue { get; init; }
        public required string Description { get; init; }
    }
}