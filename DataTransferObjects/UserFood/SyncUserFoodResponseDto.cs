using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.UserFood
{
    public record SyncUserFoodResponseDto : ISyncResponseDto<UserFoodDto>
    {
        public required DateTime LastSyncedAt { get; init; }
        public required List<UserFoodDto> UserFoods { get; init; }

        IEnumerable<UserFoodDto> ISyncResponseDto<UserFoodDto>.Entities => UserFoods;
    }
}
