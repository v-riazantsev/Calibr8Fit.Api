using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.Hubs.Abstract;
using Calibr8Fit.Api.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Calibr8Fit.Api.Hubs
{
    public class ChatHub(
        ICurrentUserService currentUserService,
        IChatService chatService
    ) : UserHubBase(currentUserService)
    {
        private readonly IChatService _chatService = chatService;

        [Authorize]
        public Task SendDirectMessage(SendDirectMessageRequestDto requestDto) =>
            WithUser(async user =>
            {
                Console.WriteLine($"[SendDirectMessage] User {user.UserName} is sending a message to {requestDto.RecipientUsername}: {requestDto.Content}");
                // Send message (and create chat if it doesn't exist)
                var result = await _chatService.SendDirectMessageAsync(requestDto, user, createChatIfNotExists: true);
                if (!result.Succeeded)
                    throw new HubException(string.Join("; ", result.Errors ?? ["Unknown error"]));
                return result.Data;
            });


        [Authorize]
        public Task SendChatMessage(SendChatMessageRequestDto requestDto) =>
            WithUser(async user =>
            {
                Console.WriteLine($"[SendChatMessage] User {user.UserName} is sending a message to {requestDto.ChatId}: {requestDto.Content}");
                // Send message (and create chat if it doesn't exist)
                var result = await _chatService.SendChatMessageAsync(requestDto, user);
                if (!result.Succeeded)
                    throw new HubException(string.Join("; ", result.Errors ?? ["Unknown error"]));
                return result.Data;
            });

        [Authorize]
        public override Task OnConnectedAsync() =>
            WithUser(async user =>
        {
            // Add user to a group
            Console.WriteLine($"[OnConnected] User connected: {user.UserName} {Context.ConnectionId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, user.UserName!);
            await base.OnConnectedAsync();
        });

        [Authorize]
        public override Task OnDisconnectedAsync(Exception? exception) =>
            WithUser(async user =>
        {
            // Remove user from a group
            Console.WriteLine($"[OnDisconnected] User disconnected: {user.UserName}");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, user.UserName!);
            await base.OnDisconnectedAsync(exception);
        });
    }
}
