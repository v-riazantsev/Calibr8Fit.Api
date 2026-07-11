using Calibr8Fit.Api.DataTransferObjects.User;

namespace Calibr8Fit.Api.DataTransferObjects.Post
{
    public record PostDto
    {
        public required Guid Id { get; init; }
        public required UserSummaryDto Author { get; init; }
        public required string Content { get; init; }
        public required IEnumerable<string> ImageUrls { get; init; }
        public DateTime CreatedAt { get; init; }
        public int CommentCount { get; init; }
        public int LikeCount { get; init; }
        public bool IsLikedByCurrentUser { get; init; }
    }
}
