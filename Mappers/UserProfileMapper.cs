using Calibr8Fit.Api.DataTransferObjects.User;
using Calibr8Fit.Api.Enums;
using Calibr8Fit.Api.Extensions;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Models;

namespace Calibr8Fit.Api.Mappers
{
    public static class UserProfileMapper
    {
        public static UserSummaryDto ToUserSummaryDto(this User user, IPathService pathService)
        {
            return new UserSummaryDto
            {
                UserName = user.UserName!,
                FirstName = user.Profile!.FirstName,
                LastName = user.Profile!.LastName,
                ProfilePictureUrl = user.GetProfilePictureUrl(pathService)
            };
        }
        public static IEnumerable<UserSummaryDto> ToUserSummaryDtos(this IEnumerable<User> users, IPathService pathService) =>
            users.Select(u => u.ToUserSummaryDto(pathService));
        public static UserProfileSettingsDto ToUserProfileSettingsDto(this User user, IPathService pathService)
        {
            return new UserProfileSettingsDto
            {
                UserName = user.UserName!,
                FirstName = user.Profile!.FirstName,
                LastName = user.Profile!.LastName,
                DateOfBirth = user.Profile!.DateOfBirth,
                Gender = user.Profile!.Gender,
                TargetWeight = user.Profile!.TargetWeight,
                Height = user.Profile!.Height,
                ActivityLevel = user.Profile!.ActivityLevel,
                Climate = user.Profile!.Climate,
                ForcedConsumptionTarget = user.Profile!.ForcedConsumptionTarget,
                ForcedBurnTarget = user.Profile!.ForcedBurnTarget,
                ForcedHydrationTarget = user.Profile!.ForcedHydrationTarget,
                ModifiedAt = user.Profile!.ModifiedAt,
                ProfilePictureUrl = user.GetProfilePictureUrl(pathService)
            };
        }
        public static UserProfileSettingsPatchDto ToUserProfileSettingsPatchDto(this User user)
        {
            return new UserProfileSettingsPatchDto
            {
                FirstName = user.Profile!.FirstName,
                LastName = user.Profile!.LastName,
                DateOfBirth = user.Profile!.DateOfBirth,
                Gender = user.Profile!.Gender,
                TargetWeight = user.Profile!.TargetWeight,
                Height = user.Profile!.Height,
                ActivityLevel = user.Profile!.ActivityLevel,
                Climate = user.Profile!.Climate,
                ForcedConsumptionTarget = user.Profile!.ForcedConsumptionTarget,
                ForcedBurnTarget = user.Profile!.ForcedBurnTarget,
                ForcedHydrationTarget = user.Profile!.ForcedHydrationTarget,
                ModifiedAt = user.Profile!.ModifiedAt
            };
        }
        public static UserProfileDto ToUserProfileDto(
            this User user,
            int friendsCount,
            int followersCount,
            int followingCount,
            FriendshipStatus friendshipStatus,
            bool followedByMe,
            IPathService pathService
            )
        {
            return new UserProfileDto
            {
                UserName = user.UserName!,
                FirstName = user.Profile!.FirstName,
                LastName = user.Profile!.LastName,
                ProfilePictureUrl = user.GetProfilePictureUrl(pathService),
                Bio = "",
                FriendsCount = friendsCount,
                FollowersCount = followersCount,
                FollowingCount = followingCount,
                FriendshipStatus = friendshipStatus,
                FollowedByMe = followedByMe
            };
        }
        //TODO: may break profile picture
        public static UserProfile ToUserProfile(
            this UserProfileSettingsPatchDto patchDto,
            User user)
        {
            return new UserProfile
            {
                Id = user.Id,
                FirstName = patchDto.FirstName ?? user.Profile!.FirstName,
                LastName = patchDto.LastName ?? user.Profile!.LastName,
                DateOfBirth = patchDto.DateOfBirth ?? user.Profile!.DateOfBirth,
                Gender = patchDto.Gender ?? user.Profile!.Gender,
                TargetWeight = patchDto.TargetWeight ?? user.Profile!.TargetWeight,
                Height = patchDto.Height ?? user.Profile!.Height,
                ActivityLevel = patchDto.ActivityLevel ?? user.Profile!.ActivityLevel,
                Climate = patchDto.Climate ?? user.Profile!.Climate,

                // Nullable fields can be set to null if explicitly provided, otherwise keep existing value
                ForcedConsumptionTarget = patchDto.ForcedConsumptionTarget,
                ForcedBurnTarget = patchDto.ForcedBurnTarget,
                ForcedHydrationTarget = patchDto.ForcedHydrationTarget,

                ModifiedAt = patchDto.ModifiedAt,
            };
        }

    }
}