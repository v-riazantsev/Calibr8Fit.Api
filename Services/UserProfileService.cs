using Calibr8Fit.Api.DataTransferObjects.User;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Services.Results;
using Microsoft.AspNetCore.JsonPatch;

namespace Calibr8Fit.Api.Services;

// Manages user profile updates, profile picture uploads, and settings synchronization
public class UserProfileService(
    IRepositoryBase<UserProfile, string> userProfileRepository,
    IUserRepositoryBase<ProfilePicture, string[]> profilePictureRepository,
    IFileService fileService,
    IPathService pathService) : IUserProfileService
{
    private readonly IRepositoryBase<UserProfile, string> _userProfileRepository = userProfileRepository;
    private readonly IUserRepositoryBase<ProfilePicture, string[]> _profilePictureRepository = profilePictureRepository;
    private readonly IFileService _fileService = fileService;
    private readonly IPathService _pathService = pathService;

    public async Task<UserProfileSettingsDto> SyncUserProfileSettingsAsync(User user, JsonPatchDocument<UserProfileSettingsPatchDto> patch)
    {
        // Apply client patches and update only if newer than database version
        var dto = user.ToUserProfileSettingsPatchDto();
        patch.ApplyTo(dto);

        // If client data is newer, update the database
        if (dto.ModifiedAt > user.Profile!.ModifiedAt)
            await _userProfileRepository.UpdateAsync(dto.ToUserProfile(user));

        // Return the latest profile settings
        return user.ToUserProfileSettingsDto(_pathService);
    }

    public async Task<Result> UploadProfilePictureAsync(User user, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Result.Failure("No file uploaded.");

        if (!_fileService.IsImage(file))
            return Result.Failure("File must be an image.");

        // Get the directory path for profile pictures
        var directoryPath = _pathService.GetProfilePictureDirectoryPath(user.UserName!);

        // Save the new profile picture
        var fileName = await _fileService.SaveImageAsync(file, directoryPath);

        // Update the user profile
        user.Profile!.ProfilePictureFileName = fileName;
        await _userProfileRepository.UpdateAsync(user.Profile);

        // Save profile picture
        var profilePicture = new ProfilePicture
        {
            UserId = user.Id,
            FileName = fileName
        };
        await _profilePictureRepository.AddAsync(profilePicture);

        return Result.Success();
    }

    public async Task<Result> DeleteProfilePictureAsync(User user, string fileName)
    {
        // Delete the file
        var path = _pathService.GetProfilePicturePath(user.UserName!, fileName);
        if (path is not null)
            _fileService.DeleteFile(path);

        // Delete from profile pictures
        await _profilePictureRepository.DeleteAsync(user.Id, fileName);

        if (user.Profile!.ProfilePictureFileName != fileName)
            return Result.Success();

        var latestPicture = await _profilePictureRepository
            .GetAllByUserIdAsync(user.Id)
            .ContinueWith(t => t.Result.OrderByDescending(p => p.FileName).FirstOrDefault());

        // Update the user profile
        await SetProfilePictureFileName(user, latestPicture?.FileName);

        return Result.Success();
    }

    public async Task<Result> DeleteMyProfilePictureAsync(User user)
    {
        if (string.IsNullOrEmpty(user.Profile!.ProfilePictureFileName))
            return Result.Failure("No profile picture found.");

        return await DeleteProfilePictureAsync(user, user.Profile.ProfilePictureFileName);
    }

    public async Task<Result> UpdateProfilePictureFileName(User user, string profilePictureFileName)
    {
        var targetPicture = await _profilePictureRepository.GetAsync(user.Id, profilePictureFileName);

        if (targetPicture is null)
            return Result.Failure("Profile picture not found.");

        await SetProfilePictureFileName(user, profilePictureFileName);

        return Result.Success();
    }

    private async Task SetProfilePictureFileName(User user, string? profilePictureFileName)
    {
        user.Profile!.ProfilePictureFileName = profilePictureFileName;
        await _userProfileRepository.UpdateAsync(user.Profile);
    }
}