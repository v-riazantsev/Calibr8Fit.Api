using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.UserMeal
{
    public record SyncUserMealsRequestDto : ISyncRequestDto<AddUserMealRequestDto>
    {
        public DateTime LastSyncedAt { get; init; } = DateTime.MinValue;

        public List<AddUserMealRequestDto> UserMeals { get; init; } = [];

        IEnumerable<AddUserMealRequestDto> ISyncRequestDto<AddUserMealRequestDto>.AddEntityRequestDtos => UserMeals;
    }
}
