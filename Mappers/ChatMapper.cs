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

        public static ChatPreviewDto ToChatPreviewDto(
            this ChatWithDetails c,
            IChatActivityTracker chatActivityTracker,
            string currentUserName,
            IPathService pathService) =>
            new()
            {
                Id = c.Chat.Id,
                DisplayName = c.Chat.Name ??
                    (!c.Chat.IsGroupChat ?
                    $"{c.DirectMember!.Profile!.FirstName} {c.DirectMember!.Profile!.LastName}"
                    : "Group Chat"),
                IsGroupChat = c.Chat.IsGroupChat,
                AvatarUrl = c.Chat.IsGroupChat
                    ? pathService.GetChatAvatarUrl(c.Chat.Id)
                    : c.DirectMember!.Profile!.ProfilePictureFileName is not null
                        ? pathService.GetProfilePictureUrl(c.DirectMember!.UserName!, c.DirectMember!.Profile!.ProfilePictureFileName)
                        : null,
                CreatedAt = c.Chat.CreatedAt,
                MemberCount = c.MemberCount,
                LastMessage = c.LastMessage?.ToChatMessageDto(currentUserName, pathService),
                LastReadByUserMessageSentAt = c.LastReadByRequesterMessageSentAt,
                LastReadByOtherMembersMessageSentAt = c.LastReadByOtherMembersMessageSentAt,
                UnreadMessagesCount = c.UnreadMessagesCount,
                DirectMember = c.DirectMember?.ToUserSummaryDto(pathService),
                TypingUsernames = chatActivityTracker.GetTypingUsernames(c.Chat.Id, currentUserName)
            };

        public static IEnumerable<ChatPreviewDto> ToChatPreviewDtos(
            this IEnumerable<ChatWithDetails> chats,
            IPathService pathService,
            IChatActivityTracker chatActivityTracker,
            string currentUserName
            ) =>
            chats.Select(c => c.ToChatPreviewDto(chatActivityTracker, currentUserName, pathService));
    }
}