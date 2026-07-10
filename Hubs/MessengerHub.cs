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
    IMessengerService messengerService
) : UserHubBase(currentUserService)
{
    private readonly IMessengerService _messengerService = messengerService;

    // SignalR groups are keyed by username so the notifier can target all active connections.
    private static string UserGroup(string username) => $"user:{username}";

    public Task<ChatMessageDto> SendDirectMessage(SendDirectMessageRequestDto dto) =>
        WithUser(async user =>
        {
            var result = await _messengerService.SendDirectMessageAsync(dto, user);

            if (!result.Succeeded)
                throw new HubException(string.Join("; ", result.Errors ?? ["Unknown error"]));

            return result.Data!.Message;
        });

    public Task<ChatMessageDto> SendChatMessage(SendChatMessageRequestDto dto) =>
        WithUser(async user =>
        {
            var result = await _messengerService.SendChatMessageAsync(dto, user);

            if (!result.Succeeded)
                throw new HubException(string.Join("; ", result.Errors ?? ["Unknown error"]));

            return result.Data!.Message;
        });

    public Task<ChatReadDto> ReadMessages(Guid fromMessageId) =>
        WithUser(async user =>
        {
            var result = await _messengerService.ReadMessagesAsync(fromMessageId, user);

            if (!result.Succeeded)
                throw new HubException(string.Join("; ", result.Errors ?? ["Unknown error"]));

            var data = result.Data!;
            return data.ReaderUpdate;
        });

    public Task OpenChat(Guid chatId) =>
        WithUser(async user =>
        {
            var result = await _messengerService.OpenChatAsync(chatId, Context.ConnectionId, user);

            if (!result.Succeeded)
                throw new HubException(string.Join("; ", result.Errors ?? ["Unknown error"]));
        });

    public Task CloseChat() =>
        WithUser(user =>
        {
            return _messengerService.CloseChatAsync(Context.ConnectionId);
        });

    public Task StartTyping(Guid chatId) =>
        WithUser(async user =>
        {
            var result = await _messengerService.StartTypingAsync(chatId, Context.ConnectionId, user);

            if (!result.Succeeded)
                throw new HubException(string.Join("; ", result.Errors ?? ["Unknown error"]));
        });

    public Task StopTyping(Guid chatId) =>
        WithUser(user =>
        {
            return _messengerService.StopTypingAsync(Context.ConnectionId, chatId);
        });

    public override Task OnConnectedAsync() =>
        WithUser(async user =>
        {
            await _messengerService.UserConnectedAsync(user.UserName!, Context.ConnectionId);

            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                UserGroup(user.UserName!));

            await base.OnConnectedAsync();
        });

    public override Task OnDisconnectedAsync(Exception? exception) =>
        WithUser(async user =>
        {
            await _messengerService.UserDisconnectedAsync(Context.ConnectionId);

            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                UserGroup(user.UserName!));

            await base.OnDisconnectedAsync(exception);
        });
}