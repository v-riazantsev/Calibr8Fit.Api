namespace Calibr8Fit.Api.DataTransferObjects.Chat.Read
{
    public record ChatReadResultDto
    {
        public required ChatReadDto ReaderUpdate { get; init; }

        public required IReadOnlyCollection<ChatReadNotificationDto> SenderUpdates { get; init; }
    }
}