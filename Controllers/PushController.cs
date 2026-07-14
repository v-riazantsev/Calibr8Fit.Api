using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.DataTransferObjects.PushToken;
using Calibr8Fit.Api.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    // Registers device push notification tokens for real-time notifications
    [ApiController]
    [Route("api/push")]
    [Authorize]
    public class PushController(
        ICurrentUserService currentUserService,
        IPushService pushService
    ) : UserControllerBase(currentUserService)
    {
        private readonly IPushService _pushService = pushService;

        [HttpPost("register")]
        public Task<IActionResult> Register([FromBody] PushTokenDto pushTokenDto) =>
            WithUserId(async userId =>
            {
                // Validate and process the push token registration
                await _pushService.RegisterPushToken(pushTokenDto, userId);
                return Ok();
            });
    }
}