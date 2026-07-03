using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Repository.Results;

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
                AvatarUrl = pathService.GetChatAvatarUrl(chat.Id),
                Members = chat.Members.Select(m => m.User!.ToUserSummaryDto(pathService)),
            };

        public static IEnumerable<ChatDto> ToChatDtos(this IEnumerable<Chat> chats, IPathService pathService) =>
            chats.Select(c => c.ToChatDto(pathService));

        public static ChatPreviewDto ToChatPreviewDto(this ChatWithDetails chatWithDetails, IPathService pathService) =>
            new()
            {
                Id = chatWithDetails.Chat.Id,
                DisplayName = chatWithDetails.Chat.Name ??
                    (!chatWithDetails.Chat.IsGroupChat ?
                    $"{chatWithDetails.DirectMember!.FirstName} {chatWithDetails.DirectMember!.LastName}"
                    : "Group Chat"),
                IsGroupChat = chatWithDetails.Chat.IsGroupChat,
                AvatarUrl = pathService.GetChatAvatarUrl(chatWithDetails.Chat.Id),
                CreatedAt = chatWithDetails.Chat.CreatedAt,
                MemberCount = chatWithDetails.MemberCount,
                LastMessage = chatWithDetails.LastMessagePreview != null ? new ChatMessagePreviewDto
                {
                    UserName = chatWithDetails.LastMessagePreview.UserName,
                    Content = chatWithDetails.LastMessagePreview.Content,
                    SentAt = chatWithDetails.LastMessagePreview.SentAt,
                    IsOwnMessage = chatWithDetails.LastMessagePreview.IsOwnMessage,
                    IsRead = chatWithDetails.LastMessagePreview.IsRead
                } : null,
                UnreadMessagesCount = chatWithDetails.UnreadMessagesCount,
            };

        public static IEnumerable<ChatPreviewDto> ToChatPreviewDtos(this IEnumerable<ChatWithDetails> chats, IPathService pathService) =>
            chats.Select(c => c.ToChatPreviewDto(pathService));
    }
}