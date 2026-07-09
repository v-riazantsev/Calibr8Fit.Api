using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.DataTransferObjects.Chat.Read;
using Calibr8Fit.Api.Hubs.Abstract;
using Calibr8Fit.Api.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Calibr8Fit.Api.Hubs;

[Authorize]
public class MessengerHub(
    ICurrentUserService currentUserService,
    IChatService chatService,
    IChatNotifier chatNotifier,
    IOnlineTracker onlineTracker,
    IChatActivityTracker chatActivityTracker
) : UserHubBase(currentUserService)
{
    private readonly IChatService _chatService = chatService;
    private readonly IChatNotifier _chatNotifier = chatNotifier;
    private readonly IOnlineTracker _onlineTracker = onlineTracker;
    private readonly IChatActivityTracker _chatActivityTracker = chatActivityTracker;

    private static string UserGroup(string username) => $"user:{username}";

    public Task<ChatMessageDto> SendDirectMessage(SendDirectMessageRequestDto dto) =>
        WithUser(async user =>
        {
            var result = await _chatService.SendDirectMessageAsync(
                dto,
                user,
                createChatIfNotExists: true);

            if (!result.Succeeded)
                throw new HubException(string.Join("; ", result.Errors ?? ["Unknown error"]));

            var message = result.Data!.Message;

            await _chatNotifier.NotifyMessageSentAsync(
                user.UserName!,
                message);

            await _chatNotifier.NotifyMessageIncomingAsync(
                dto.RecipientUsername,
                message);

            return message;
        });

    public Task<ChatMessageDto> SendChatMessage(SendChatMessageRequestDto dto) =>
        WithUser(async user =>
        {
            var result = await _chatService.SendChatMessageAsync(dto, user);

            if (!result.Succeeded)
                throw new HubException(string.Join("; ", result.Errors ?? ["Unknown error"]));

            var response = result.Data!;

            await _chatNotifier.NotifyMessageSentAsync(
                user.UserName!,
                response.Message);

            foreach (var username in response.RecipientUsernames)
            {
                await _chatNotifier.NotifyMessageIncomingAsync(
                    username,
                    response.Message);
            }

            return response.Message;
        });

    public Task<ChatReadDto> ReadMessages(Guid fromMessageId) =>
        WithUser(async user =>
        {
            var result =
                await _chatService.UpdateChatReadAsync(fromMessageId, user.Id);

            if (!result.Succeeded)
                throw new HubException(string.Join("; ", result.Errors ?? ["Unknown error"]));

            foreach (var notification in result.Data!.SenderUpdates)
            {
                await _chatNotifier.NotifyMessagesReadAsync(
                    notification.SenderUsername,
                    notification.Read);
            }

            return result.Data.ReaderUpdate;
        });

    public Task OpenChat(Guid chatId) =>
        WithUser(async user =>
        {
            var result =
                await _chatService.EnsureUserIsChatMemberAsync(chatId, user.Id);

            if (!result.Succeeded)
                throw new HubException(string.Join("; ", result.Errors ?? ["Unknown error"]));

            _chatActivityTracker.OpenChat(
                user.UserName!,
                Context.ConnectionId,
                chatId);
        });

    public Task CloseChat() =>
        WithUser(user =>
        {
            _chatActivityTracker.CloseChat(Context.ConnectionId);
            return Task.CompletedTask;
        });

    public Task StartTyping(Guid chatId) =>
        WithUser(async user =>
        {
            var result =
                await _chatService.EnsureUserIsChatMemberAsync(chatId, user.Id);

            if (!result.Succeeded)
                throw new HubException(string.Join("; ", result.Errors ?? ["Unknown error"]));


            _chatActivityTracker.StartTyping(
                user.UserName!,
                Context.ConnectionId,
                chatId);

            // TODO:
            // Notify other members typing started.
        });

    public Task StopTyping(Guid chatId) =>
        WithUser(user =>
        {
            _chatActivityTracker.StopTyping(
                Context.ConnectionId,
                chatId);

            // TODO:
            // Notify other members typing stopped.

            return Task.CompletedTask;
        });

    public override Task OnConnectedAsync() =>
        WithUser(async user =>
        {
            _onlineTracker.UserConnected(
                user.UserName!,
                Context.ConnectionId);

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                UserGroup(user.UserName!));

            await base.OnConnectedAsync();
        });

    public override Task OnDisconnectedAsync(Exception? exception) =>
        WithUser(async user =>
        {
            _chatActivityTracker.CloseChat(Context.ConnectionId);

            _onlineTracker.UserDisconnected(
                Context.ConnectionId);

            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                UserGroup(user.UserName!));

            await base.OnDisconnectedAsync(exception);
        });
}