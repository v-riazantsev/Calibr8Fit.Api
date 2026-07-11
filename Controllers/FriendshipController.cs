using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    [ApiController]
    [Route("api/friendship")]
    [Authorize]
    public class FriendshipController(
        ICurrentUserService currentUserService,
        IFriendshipService friendshipService
        ) : UserControllerBase(currentUserService)
    {
        private readonly IFriendshipService _friendshipService = friendshipService;

        [HttpPost("request/{addresseeUsername}")]
        public Task<IActionResult> SendFriendRequest(string addresseeUsername) =>
            WithUserId(async userId =>
            {
                var result = await _friendshipService.SendFriendRequestAsync(userId, addresseeUsername);

                return result.Succeeded
                    ? Ok(result.Data)
                    : BadRequest(new { errors = result.Errors });
            });

        [HttpPost("request/{requesterUsername}/accept")]
        public Task<IActionResult> AcceptFriendRequest(string requesterUsername) =>
            WithUserId(async userId =>
            {
                var result = await _friendshipService.AcceptFriendRequestAsync(userId, requesterUsername);

                return result.Succeeded
                    ? Ok(result.Data)
                    : BadRequest(new { errors = result.Errors });
            });

        [HttpDelete("request/{requesterUsername}/reject")]
        public Task<IActionResult> RejectFriendRequest(string requesterUsername) =>
            WithUserId(async userId =>
            {
                var result = await _friendshipService.RejectFriendRequestAsync(userId, requesterUsername);

                return result.Succeeded
                    ? Ok(new { message = "Friend request rejected" })
                    : BadRequest(new { errors = result.Errors });
            });

        [HttpDelete("request/{addresseeUsername}/cancel")]
        public Task<IActionResult> CancelFriendRequest(string addresseeUsername) =>
            WithUserId(async userId =>
            {
                var result = await _friendshipService.CancelFriendRequestAsync(userId, addresseeUsername);

                return result.Succeeded
                    ? Ok(new { message = "Friend request cancelled" })
                    : BadRequest(new { errors = result.Errors });
            });

        [HttpGet("requests/pending")]
        public Task<IActionResult> GetPendingFriendRequests() =>
            WithUserId(async userId =>
            {
                var requests = await _friendshipService.GetPendingFriendRequestsAsync(userId);
                return Ok(requests);
            });

        [HttpGet("requests/sent")]
        public Task<IActionResult> GetSentFriendRequests() =>
            WithUserId(async userId =>
            {
                var requests = await _friendshipService.GetSentFriendRequestsAsync(userId);
                return Ok(requests);
            });

        // Friendship Management

        [HttpDelete("{friendUsername}")]
        public Task<IActionResult> RemoveFriendship(string friendUsername) =>
            WithUserName(async username =>
            {
                var result = await _friendshipService.RemoveFriendshipAsync(username, friendUsername);

                return result.Succeeded
                    ? Ok(new { message = "Friendship removed" })
                    : BadRequest(new { errors = result.Errors });
            });

        [HttpGet("{username}/are-friends")]
        public Task<IActionResult> AreFriends(string username) =>
            WithUserName(async currentUsername =>
            {
                var areFriends = await _friendshipService.AreFriendsAsync(currentUsername, username);
                return Ok(new { areFriends });
            });

        [HttpGet("friends")]
        public Task<IActionResult> GetAllFriends() =>
            WithUserId(async userId =>
            {
                var friendships = await _friendshipService.GetUserFriendshipsAsync(userId);
                return Ok(friendships);
            });
        [HttpGet("friends/search")]
        public Task<IActionResult> SearchFriends(
            [FromQuery] string query = "",
            [FromQuery] int page = 0,
            [FromQuery] int size = 10) =>
            WithUserName(async username =>
            {
                var friendships = await _friendshipService.SearchFriendshipsOfUserAsync(username, query, page, size);
                return Ok(friendships);
            });

        [HttpGet("{username}/friends")]
        public async Task<IActionResult> GetFriendsOfUser(string username) =>
            Ok(await _friendshipService.GetUserFriendshipsAsyncByUsername(username));

        [HttpGet("{username}/friends/search")]
        public async Task<IActionResult> SearchFriendsOfUser(
            [FromRoute] string username,
            [FromQuery] string query = "",
            [FromQuery] int page = 0,
            [FromQuery] int size = 10) =>
            Ok(await _friendshipService.SearchFriendshipsOfUserAsync(username, query, page, size));
    }
}