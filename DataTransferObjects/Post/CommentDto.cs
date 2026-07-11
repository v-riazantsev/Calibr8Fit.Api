using Calibr8Fit.Api.DataTransferObjects.User;

namespace Calibr8Fit.Api.DataTransferObjects.Post
{
    public record CommentDto
    {
        public required Guid Id { get; init; }
        public required Guid PostId { get; init; }
        public required UserSummaryDto Author { get; init; }
        public required string Content { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}