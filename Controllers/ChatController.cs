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
        IChatNotifier chatNotifier,
        ICurrentUserService currentUserService
        ) : UserControllerBase(currentUserService)
    {
        private readonly IChatService _chatService = chatService;
        private readonly IChatNotifier _chatNotifier = chatNotifier;

        [HttpPost("chat-message")]
        public Task<IActionResult> SendChatMessage([FromBody] SendChatMessageRequestDto requestDto) =>
            WithUser(async user =>
            {
                var result = await _chatService.SendChatMessageAsync(requestDto, user);
                return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
            });

        [HttpPost("direct")]
        public Task<IActionResult> SendDirectMessage([FromBody] SendDirectMessageRequestDto dto) =>
            WithUser(async user =>
            {
                var result = await _chatService.SendDirectMessageAsync(
                    dto,
                    user,
                    createChatIfNotExists: true);

                if (!result.Succeeded)
                    return BadRequest(string.Join("; ", result.Errors ?? ["Unknown error"]));

                var message = result.Data!.Message;

                await _chatNotifier.NotifyMessageSentAsync(
                    user.UserName!,
                    message);

                await _chatNotifier.NotifyMessageIncomingAsync(
                    dto.RecipientUsername,
                    message);

                return Ok(result.Data);
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
        [HttpPost("read")]
        public Task ReadMessages(Guid fromMessageId) =>
            WithUser(async user =>
            {
                var result = await _chatService.UpdateChatReadAsync(fromMessageId, user.Id);
                return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
            });
    }
}