using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.UserFood
{
    public record SyncUserFoodsRequestDto : ISyncRequestDto<AddUserFoodRequestDto>
    {
        public DateTime LastSyncedAt { get; init; } = DateTime.MinValue;

        public List<AddUserFoodRequestDto> UserFoods { get; init; } = [];

        IEnumerable<AddUserFoodRequestDto> ISyncRequestDto<AddUserFoodRequestDto>.AddEntityRequestDtos => UserFoods;
    }
}
