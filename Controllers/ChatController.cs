using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController(
        IChatService chatService,
        ICurrentUserService currentUserService
        ) : UserControllerBase(currentUserService)
    {
        private readonly IChatService _chatService = chatService;

        [HttpPost("chat-message")]
        public Task<IActionResult> SendChatMessage([FromBody] SendChatMessageRequestDto requestDto) =>
            WithUser(async user =>
            {
                var result = await _chatService.SendChatMessageAsync(requestDto, user);
                return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
            });

        [HttpPost("direct")]
        public Task<IActionResult> SendDirectMessage([FromBody] SendDirectMessageRequestDto requestDto) =>
            WithUser(async user =>
            {
                var result = await _chatService.SendDirectMessageAsync(requestDto, user, createChatIfNotExists: true);
                return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
            });

        [HttpGet("direct/{username}")]
        public Task<IActionResult> GetDirectMessagesWithUser(
            string username,
            [FromQuery] Guid? before = null,
            [FromQuery] int size = 100
            ) =>
            WithUserId(async userId =>
            {
                var result = await _chatService.GetDirectMessagesAsync(userId, username, before, size);
                return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
            });

        [HttpGet]
        public Task<IActionResult> GetUserChats() =>
            WithUserId(async userId =>
            {
                var result = await _chatService.GetUserChatsAsync(userId);
                return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
            });
        [HttpGet("messages")]
        public Task<IActionResult> GetChatMessages(
            [FromQuery] Guid chatId,
            [FromQuery] Guid? before = null,
            [FromQuery] int size = 100
            ) =>
            WithUserId(async userId =>
            {
                Console.WriteLine($"GetChatMessages called with chatId: {chatId}, before: {before}, size: {size}");
                var result = await _chatService.GetChatMessagesAsync(chatId, userId, before, size);
                return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
            });
    }
}