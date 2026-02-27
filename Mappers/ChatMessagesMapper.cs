using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.Models;

namespace Calibr8Fit.Api.Mappers
{
    public static class ChatMessagesMapper
    {
        public static ChatMessage ToChatMessage(this SendChatMessageRequestDto request, string senderUserId) =>
            new()
            {
                Id = request.Id,
                ChatId = request.ChatId,
                UserId = senderUserId,
                Content = request.Content,
                SentAt = request.SendedAt
            };

        public static ChatMessage ToChatMessage(this SendDirectMessageRequestDto request, string senderUserId, Guid chatId) =>
            new()
            {
                ChatId = chatId,
                UserId = senderUserId,
                Content = request.Content,
                SentAt = request.SendedAt
            };
        public static ChatMessageDto ToChatMessageDto(this ChatMessage message) =>
            new()
            {
                Id = message.Id,
                ChatId = message.ChatId,
                SenderUsername = message.User?.UserName ?? "Unknown",
                Content = message.Content,
                SendedAt = message.SentAt,
                ReadAt = message.ReadAt
            };

        public static IEnumerable<ChatMessageDto> ToChatMessageDtos(this IEnumerable<ChatMessage> messages) =>
            messages.Select(m => m.ToChatMessageDto());

    }
}