using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.UserMeal
{
    public record SyncUserMealResponseDto : ISyncResponseDto<UserMealDto>
    {
        public required DateTime LastSyncedAt { get; init; }
        public required List<UserMealDto> UserMeals { get; init; }

        IEnumerable<UserMealDto> ISyncResponseDto<UserMealDto>.Entities => UserMeals;
    }
}
