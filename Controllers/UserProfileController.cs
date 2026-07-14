using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.DataTransferObjects.User;
using Calibr8Fit.Api.Enums;
using Calibr8Fit.Api.Extensions;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    // Manages user profile information, picture uploads, and relationship statuses
    [ApiController]
    [Route("api/user-profile")]
    public class UserProfileController(
        ICurrentUserService currentUserService,
        IRepositoryBase<UserProfile, string> userProfileRepository,
        IUserRepository userRepository,
        IFriendshipService friendshipService,
        IFollowingService followingService,
        IUserProfileService userProfileService,
        IPathService pathService
        ) : UserControllerBase(currentUserService)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRepositoryBase<UserProfile, string> _userProfileRepository = userProfileRepository;
        private readonly IFriendshipService _friendshipService = friendshipService;
        private readonly IFollowingService _followingService = followingService;
        private readonly IUserProfileService _userProfileService = userProfileService;
        private readonly IPathService _pathService = pathService;

        [HttpGet]
        [Authorize]
        public Task<IActionResult> GetMyProfile() =>
            WithUser(user =>
                user?.Profile is null
                    ? Unauthorized("User profile not found.")
                    : Ok(user.ToUserProfileDto(
                        _friendshipService.GetFriendsCountAsync(user.Id).Result,
                        _followingService.GetFollowersCountAsync(user.Id).Result,
                        _followingService.GetFollowingCountAsync(user.Id).Result,
                        FriendshipStatus.None, // Own profile, no friendship status
                        false, // Own profile, cannot be followed by self
                        _pathService
                    ))
            );

        [HttpGet("{username}")]
        [Authorize]
        public Task<IActionResult> GetUserProfileByUsername(string username) =>
            WithUserId(async thisUserId =>
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                if (user?.Profile is null)
                    return NotFound($"User or user profile with {username} not found.");

                // Fetch counts and friendship status
                return Ok(user.ToUserProfileDto(
                    _friendshipService.GetFriendsCountAsync(user.Id).Result,
                    _followingService.GetFollowersCountAsync(user.Id).Result,
                    _followingService.GetFollowingCountAsync(user.Id).Result,
                    _friendshipService.GetFriendshipStatusAsync(thisUserId, username).Result,
                    _followingService.IsFollowingAsync(thisUserId, username).Result,
                    _pathService
                ));
            });

        [HttpGet("settings")]
        [Authorize]
        public Task<IActionResult> GetMyProfileSettings() =>
            WithUser(user =>
                user?.Profile is null
                    ? Unauthorized("User profile not found.")
                    : Ok(user.ToUserProfileSettingsDto(_pathService))
            );

        [HttpPut("settings")]
        [Authorize]
        public Task<IActionResult> SyncMyProfileSettingsAsync([FromBody] JsonPatchDocument<UserProfileSettingsPatchDto> patch) =>
            WithUser(async user =>
            {
                if (user?.Profile is null) return Unauthorized("User profile not found.");

                // Check if the patch has the required 'modifiedAt'
                if (!patch.Operations.Any(op =>
                    op.path.Equals("/modifiedAt", StringComparison.OrdinalIgnoreCase) &&
                    op.OperationType == OperationType.Replace))
                    return BadRequest("Patch must include '/modifiedAt'.");

                // Sync profile settings and return the latest data
                return Ok(await _userProfileService.SyncUserProfileSettingsAsync(user, patch));
            });

        // Profile Picture Management

        [HttpPost("profile-picture")]
        [Authorize]
        public Task<IActionResult> UploadProfilePicture(IFormFile file) =>
            WithUser(async user =>
            {
                var result = await _userProfileService.UploadProfilePictureAsync(user, file);

                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                return Ok(user.GetProfilePictureUrl(_pathService));
            });

        [HttpDelete("profile-picture")]
        [Authorize]
        public Task<IActionResult> DeleteMyProfilePicture() =>
            WithUser(async user =>
            {
                var result = await _userProfileService.DeleteMyProfilePictureAsync(user);

                if (!result.Succeeded)
                    return BadRequest(result.Errors?.FirstOrDefault() ?? "Unknown error");

                return Ok(new { message = "Profile picture deleted successfully." });
            });
    }
}