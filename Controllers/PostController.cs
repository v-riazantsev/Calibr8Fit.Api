using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.DataTransferObjects.Post;
using Calibr8Fit.Api.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    // Manages user posts with likes, comments, and image uploads
    [ApiController]
    [Route("api/post")]
    [Authorize]
    public class PostController(
        ICurrentUserService currentUserService,
        IPostService postService
        ) : UserControllerBase(currentUserService)
    {
        private readonly IPostService _postService = postService;

        [HttpPost]
        [Consumes("multipart/form-data")]
        public Task<IActionResult> CreatePost([FromForm] CreatePostRequestDto createPostRequestDto) =>
            WithUserId(async userId =>
            {
                var result = await _postService.CreatePostAsync(createPostRequestDto, userId);
                return result.Succeeded
                    ? CreatedAtAction(nameof(GetPost), new { postId = result.Data!.Id }, result.Data)
                    : BadRequest(new { errors = result.Errors });
            });

        [HttpGet("{postId}")]
        public Task<IActionResult> GetPost(Guid postId) =>
            WithUserId(async userId =>
            {
                var result = await _postService.GetPostAsync(postId, userId);
                return result.Succeeded
                    ? Ok(result.Data)
                    : NotFound(new { errors = result.Errors });
            });

        [HttpGet("my")]
        public Task<IActionResult> GetMyPosts(
            [FromQuery] int page = 0,
            [FromQuery] int size = 10) =>
            WithUserId(async userId =>
            {
                var result = await _postService.GetLatestPostsByUserIdAsync(userId, page, size, userId);
                return result.Succeeded
                    ? Ok(result.Data)
                    : NotFound(new { errors = result.Errors });
            });

        [HttpGet("feed")]
        public Task<IActionResult> GetFeedPosts(
            [FromQuery] int page = 0,
            [FromQuery] int size = 10) =>
            WithUser(async user =>
            {
                var result = await _postService.GetFeedPostsAsync(user, page, size);
                return result.Succeeded
                    ? Ok(result.Data)
                    : NotFound(new { errors = result.Errors });
            });

        [HttpGet("user/{username}")]
        public Task<IActionResult> GetPostsByUser(
            string username,
            [FromQuery] int page = 0,
            [FromQuery] int size = 10) =>
            WithUserId(async userId =>
            {
                var result = await _postService.GetLatestPostsByUserNameAsync(username, page, size, userId);
                return result.Succeeded
                    ? Ok(result.Data)
                    : NotFound(new { errors = result.Errors });
            });

        [HttpDelete("{postId}")]
        public Task<IActionResult> DeletePost(Guid postId) =>
            WithUserId(async userId =>
            {
                var result = await _postService.DeletePostAsync(postId, userId);
                return result.Succeeded
                    ? NoContent()
                    : BadRequest(new { errors = result.Errors });
            });

        [HttpPost("{postId}/like")]
        public Task<IActionResult> LikePost(Guid postId) =>
            WithUserId(async userId =>
            {
                var result = await _postService.LikePostAsync(postId, userId);
                return result.Succeeded
                    ? Ok()
                    : BadRequest(new { errors = result.Errors });
            });

        [HttpDelete("{postId}/like")]
        public Task<IActionResult> UnlikePost(Guid postId) =>
            WithUserId(async userId =>
            {
                var result = await _postService.UnlikePostAsync(postId, userId);
                return result.Succeeded
                    ? NoContent()
                    : BadRequest(new { errors = result.Errors });
            });

        [HttpGet("{postId}/comments")]
        public async Task<IActionResult> GetComments(
            Guid postId,
            [FromQuery] int page = 0,
            [FromQuery] int size = 10)
        {
            var result = await _postService.GetCommentsAsync(postId, page, size);
            return result.Succeeded
                ? Ok(result.Data)
                : NotFound(new { errors = result.Errors });
        }

        [HttpPost("{postId}/comment")]
        public Task<IActionResult> AddComment(Guid postId, [FromBody] string content) =>
            WithUserId(async userId =>
            {
                var result = await _postService.AddCommentAsync(postId, content, userId);
                return result.Succeeded
                    ? CreatedAtAction(nameof(GetPost), new { postId }, result.Data)
                    : BadRequest(new { errors = result.Errors });
            });

        [HttpDelete("comment/{commentId}")]
        public Task<IActionResult> DeleteComment(Guid commentId) =>
            WithUserId(async userId =>
            {
                var result = await _postService.DeleteCommentAsync(commentId, userId);
                return result.Succeeded
                    ? NoContent()
                    : BadRequest(new { errors = result.Errors });
            });
    }
}
