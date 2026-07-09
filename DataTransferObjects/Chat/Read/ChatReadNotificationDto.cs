namespace Calibr8Fit.Api.DataTransferObjects.Chat.Read
{
    public class ChatReadNotificationDto
    {
        public required string SenderUsername { get; init; }

        public required ChatReadDto Read { get; init; }
    }
}