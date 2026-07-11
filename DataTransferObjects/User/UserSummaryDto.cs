namespace Calibr8Fit.Api.DataTransferObjects.User
{
    public record UserSummaryDto
    {
        public required string UserName { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public string? ProfilePictureUrl { get; init; }
    }
}