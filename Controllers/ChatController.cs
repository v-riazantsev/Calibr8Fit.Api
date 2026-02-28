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
            [FromQuery] int page = 0,
            [FromQuery] int size = 100
            ) =>
            WithUserId(async userId =>
            {
                var result = await _chatService.GetDirectMessagesAsync(userId, username, page, size);
                return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
            });

        [HttpGet("direct")]
        public Task<IActionResult> GetDirectChats() =>
            WithUserId(async userId =>
            {
                var result = await _chatService.GetDirectChatsAsync(userId);
                return result.Succeeded ? Ok(result.Data) : BadRequest(result.Errors);
            });
    }
}