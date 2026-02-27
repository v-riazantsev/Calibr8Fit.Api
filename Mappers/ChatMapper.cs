using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Models;

namespace Calibr8Fit.Api.Mappers
{
    public static class ChatMapper
    {
        public static ChatDto ToChatDto(this Chat chat, IPathService pathService) =>
            new()
            {
                Id = chat.Id,
                Name = chat.Name,
                IsGroupChat = chat.IsGroupChat,
                CreatedAt = chat.CreatedAt,
                Members = chat.Members.Select(m => m.User!.ToUserSummaryDto(pathService))
            };

        public static IEnumerable<ChatDto> ToChatDtos(this IEnumerable<Chat> chats, IPathService pathService) =>
            chats.Select(c => c.ToChatDto(pathService));
    }
}