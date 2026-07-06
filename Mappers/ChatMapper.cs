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

        public static ChatPreviewDto ToChatPreviewDto(this ChatWithDetails c, IPathService pathService) =>
            new()
            {
                Id = c.Chat.Id,
                DisplayName = c.Chat.Name ??
                    (!c.Chat.IsGroupChat ?
                    $"{c.DirectMember!.FirstName} {c.DirectMember!.LastName}"
                    : "Group Chat"),
                IsGroupChat = c.Chat.IsGroupChat,
                AvatarUrl = c.Chat.IsGroupChat
                    ? pathService.GetChatAvatarUrl(c.Chat.Id)
                    : c.DirectMember!.ProfilePictureFileName is not null
                        ? pathService.GetProfilePictureUrl(c.DirectMember!.UserName!, c.DirectMember!.ProfilePictureFileName!)
                        : null,
                CreatedAt = c.Chat.CreatedAt,
                MemberCount = c.MemberCount,
                LastMessage = c.LastMessagePreview != null ? new ChatMessagePreviewDto
                {
                    SenderUserName = c.LastMessagePreview.UserName,
                    Content = c.LastMessagePreview.Content,
                    SentAt = c.LastMessagePreview.SentAt,
                    IsOwnMessage = c.LastMessagePreview.IsOwnMessage,
                    IsReadByUser = c.LastMessagePreview.IsReadByRequester,
                    IsReadByOthers = c.LastMessagePreview.IsReadByOthers
                } : null,
                LastReadMessageId = c.LastReadMessageId,
                UnreadMessagesCount = c.UnreadMessagesCount,
            };

        public static IEnumerable<ChatPreviewDto> ToChatPreviewDtos(this IEnumerable<ChatWithDetails> chats, IPathService pathService) =>
            chats.Select(c => c.ToChatPreviewDto(pathService));
    }
}