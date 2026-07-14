using Calibr8Fit.Api.DataTransferObjects.Authentication;
using Calibr8Fit.Api.DataTransferObjects.Token;
using Calibr8Fit.Api.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Calibr8Fit.Api.Controllers.Abstract;

namespace Calibr8Fit.Api.Controllers
{
    // Handles user authentication, registration, and token management
    [Route("api/auth")]
    [ApiController]
    public class AuthController(
        IAuthService authService,
        ICurrentUserService currentUserService
        ) : UserControllerBase(currentUserService)
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
        {
            // Return user creation result
            var result = await _authService.RegisterUserAsync(registerDto);
            return result.Succeeded
                ? Ok(result.Data)
                : BadRequest(result.Errors);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            // Return login result
            var result = await _authService.LoginUserAsync(loginDto);
            return result.Succeeded
                ? Ok(result.Data)
                : Unauthorized(result.Errors);
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto refreshTokenDto)
        {
            // Return refresh token result
            var result = await _authService.RefreshTokenAsync(refreshTokenDto);
            return result.Succeeded
                ? Ok(result.Data)
                : Unauthorized(result.Errors);
        }
        [HttpPost("logout")]
        [Authorize]
        public Task<IActionResult> Logout([FromQuery] string deviceId) =>
            WithUserId(async userId =>
            {
                var result = await _authService.LogoutAsync(userId, deviceId);
                return result.Succeeded
                    ? NoContent()
                    : NotFound(result.Errors);
            });
        [HttpPost("logout-all")]
        [Authorize]
        public Task<IActionResult> LogoutAll() =>
            WithUserId(async userId =>
            {
                var result = await _authService.LogoutAllAsync(userId);
                return result.Succeeded
                    ? NoContent()
                    : NotFound(result.Errors);
            });
    }
}