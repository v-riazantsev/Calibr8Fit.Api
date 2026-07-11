using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    [Route("api/users")]
    [Authorize]
    [ApiController]
    public class UserController(
        IUserRepository userRepo,
        ICurrentUserService currentUserService,
        IFileService fileService,
        IPathService pathService
        ) : UserControllerBase(currentUserService)
    {
        private readonly IUserRepository _userRepo = userRepo;
        private readonly IPathService _pathService = pathService;
        private readonly IFileService _fileService = fileService;

        [HttpGet("me")]
        public Task<IActionResult> GetMySummary() =>
            WithUser(user => Ok(user.ToUserSummaryDto(_pathService)));

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] string query,
            [FromQuery] int page = 0,
            [FromQuery] int size = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query cannot be empty.");

            var users = await _userRepo.SearchByUsernameAsync(query, page, size);

            var result = users.Select(u => u.ToUserSummaryDto(_pathService));

            return Ok(result);
        }
        [HttpGet("{username}")]
        public async Task<IActionResult> GetSummary(string username)
        {
            // Get user by username
            var user = await _userRepo.GetByUsernameAsync(username);
            if (user is null) return NotFound();

            // Return user summary
            return Ok(user.ToUserSummaryDto(_pathService));
        }
        [HttpDelete("me")]
        public Task<IActionResult> DeleteMe() =>
            WithUserName(DeleteUser);

        [HttpDelete("{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            // Get user by username
            var user = await _userRepo.GetByUsernameAsync(username);
            if (user?.UserName is null) return NotFound();

            // delete user directory
            var userDirectory = _pathService.GetUserUploadsDirectoryPath(user.UserName);
            if (userDirectory is not null) _fileService.DeleteDirectory(userDirectory);

            // delete user from DB
            await _userRepo.DeleteAsync(user.Id);

            return NoContent();
        }
    }
}