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

    public override Task OnConnectedAsync() =>
        WithUser(async user =>
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                UserGroup(user.UserName!));

            await base.OnConnectedAsync();
        });

    public override Task OnDisconnectedAsync(Exception? exception) =>
        WithUser(async user =>
        {
            await Groups.RemoveFromGroupAsync(
                Context.ConnectionId,
                UserGroup(user.UserName!));

            await base.OnDisconnectedAsync(exception);
        });
}