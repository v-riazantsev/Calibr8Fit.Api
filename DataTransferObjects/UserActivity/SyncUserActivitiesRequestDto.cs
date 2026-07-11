using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.UserActivity
{
    public record SyncUserActivitiesRequestDto : ISyncRequestDto<AddUserActivityRequestDto>
    {
        public DateTime LastSyncedAt { get; init; } = DateTime.MinValue;

        public List<AddUserActivityRequestDto> UserActivities { get; init; } = [];

        IEnumerable<AddUserActivityRequestDto> ISyncRequestDto<AddUserActivityRequestDto>.AddEntityRequestDtos => UserActivities;
    }
}