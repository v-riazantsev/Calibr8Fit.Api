using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    // Manages user follow relationships and follower/following lists
    [ApiController]
    [Route("api/follow")]
    [Authorize]
    public class FollowController(
        ICurrentUserService currentUserService,
        IFollowingService followingService,
        IUserRepository userRepository
        ) : UserControllerBase(currentUserService)
    {
        private readonly IFollowingService _followingService = followingService;
        private readonly IUserRepository _userRepository = userRepository;

        [HttpPost("{username}")]
        public Task<IActionResult> FollowUser(string username) =>
            WithUserId(async userId =>
            {
                var result = await _followingService.FollowUserAsync(userId, username);
                return result.Succeeded
                    ? Ok(new { message = "Successfully followed user" })
                    : BadRequest(new { errors = result.Errors });
            });

        [HttpDelete("{username}")]
        public Task<IActionResult> UnfollowUser(string username) =>
            WithUserId(async userId =>
            {
                var result = await _followingService.UnfollowUserAsync(userId, username);
                return result.Succeeded
                    ? Ok(new { message = "Successfully unfollowed user" })
                    : BadRequest(new { errors = result.Errors });
            });

        [HttpGet("{username}/followers")]
        public async Task<IActionResult> GetFollowers(string username)
        {
            var userId = await _userRepository.GetIdByUsernameAsync(username);
            if (userId is null) return NotFound(new { errors = new[] { "User not found" } });

            var result = await _followingService.GetFollowersAsync(userId);
            return result.Succeeded
                ? Ok(result.Data)
                : BadRequest(new { errors = result.Errors });
        }

        [HttpGet("{username}/followers/search")]
        public async Task<IActionResult> SearchFollowers(
            [FromRoute] string username,
            [FromQuery] string query = "",
            [FromQuery] int page = 0,
            [FromQuery] int size = 10)
        {
            var userId = await _userRepository.GetIdByUsernameAsync(username);
            if (userId is null) return NotFound(new { errors = new[] { "User not found" } });

            var result = await _followingService.SearchFollowersAsync(userId, query, page, size);
            return result.Succeeded
                ? Ok(result.Data)
                : BadRequest(new { errors = result.Errors });
        }

        [HttpGet("{username}/following")]
        public async Task<IActionResult> GetFollowing(string username)
        {
            var userId = await _userRepository.GetIdByUsernameAsync(username);
            if (userId is null) return NotFound(new { errors = new[] { "User not found" } });

            var result = await _followingService.GetFollowingAsync(userId);
            return result.Succeeded
                ? Ok(result.Data)
                : BadRequest(new { errors = result.Errors });
        }

        [HttpGet("{username}/following/search")]
        public async Task<IActionResult> SearchFollowing(
            [FromRoute] string username,
            [FromQuery] string query = "",
            [FromQuery] int page = 0,
            [FromQuery] int size = 10)
        {
            var userId = await _userRepository.GetIdByUsernameAsync(username);
            if (userId is null) return NotFound(new { errors = new[] { "User not found" } });

            var result = await _followingService.SearchFollowingAsync(userId, query, page, size);
            return result.Succeeded
                ? Ok(result.Data)
                : BadRequest(new { errors = result.Errors });
        }
    }
}