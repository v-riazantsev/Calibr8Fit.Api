namespace Calibr8Fit.Api.DataTransferObjects.Chat.Read
{
    public record ChatReadDto(
        Guid ChatId,
        DateTime FromMessageSentAt
    );
}