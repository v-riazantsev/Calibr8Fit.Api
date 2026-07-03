namespace Calibr8Fit.Api.Repository.Results
{
    public class ChatMessageDetailed
    {
        public required Guid Id { get; init; }
        public required Guid ChatId { get; init; }
        public required string SenderUserId { get; init; }
        public required string SenderUserName { get; init; }
        public required string SenderFirstName { get; init; }
        public required string SenderLastName { get; init; }
        public required string? SenderProfilePictureFileName { get; init; }
        public required string Content { get; init; }
        public required DateTime SentAt { get; init; }
        public required bool IsOwnMessage { get; init; }
        public required bool IsReadByRequester { get; init; }
        public required bool IsReadByOthers { get; init; }
    }
}