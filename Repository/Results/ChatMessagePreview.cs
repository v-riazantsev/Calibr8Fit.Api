namespace Calibr8Fit.Api.Repository.Results
{
    public class ChatMessagePreview
    {
        public required string UserName { get; init; }
        public required string Content { get; init; }
        public required DateTime SentAt { get; init; }
        public required bool IsOwnMessage { get; init; }
        public required bool IsReadByRequester { get; init; }
        public required bool IsReadByOthers { get; init; }
    }
}