using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.DataTransferObjects.User;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Repository.Results;

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
                SentAt = request.SentAt
            };

        public static ChatMessage ToChatMessage(this SendDirectMessageRequestDto request, string senderUserId, Guid chatId) =>
            new()
            {
                ChatId = chatId,
                UserId = senderUserId,
                Content = request.Content,
                SentAt = request.SendedAt
            };
        public static ChatMessageDto ToChatMessageDto(this ChatMessageDetailed message, string currentUserId, IPathService pathService) =>
            new()
            {
                Id = message.Id,
                ChatId = message.ChatId,
                Sender = new UserSummaryDto
                {
                    UserName = message.SenderUserName,
                    FirstName = message.SenderFirstName,
                    LastName = message.SenderLastName,
                    ProfilePictureUrl = message.SenderProfilePictureFileName is not null
                        ? pathService.GetProfilePictureUrl(message.SenderUserName, message.SenderProfilePictureFileName)
                        : null
                },
                Content = message.Content,
                SentAt = message.SentAt,
                IsOwnMessage = message.IsOwnMessage,
                IsReadByUser = message.IsReadByRequester,
                IsReadByOthers = message.IsReadByOthers
            };

        public static IEnumerable<ChatMessageDto> ToChatMessageDtos(
            this IEnumerable<ChatMessageDetailed> messages,
            string currentUserId,
            IPathService pathService)
            => messages.Select(m => m.ToChatMessageDto(currentUserId, pathService));

    }
}