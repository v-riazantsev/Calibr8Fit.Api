namespace Calibr8Fit.Api.DataTransferObjects.Chat.Read
{
    public record ChatReadNotificationDto
    {
        public required string SenderUsername { get; init; }

        public required ChatReadDto Read { get; init; }
    }
}