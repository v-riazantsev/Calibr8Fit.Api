using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.UserActivity
{
    public record SyncUserActivityResponseDto : ISyncResponseDto<UserActivityDto>
    {
        public required DateTime LastSyncedAt { get; init; }
        public required List<UserActivityDto> UserActivities { get; init; }

        IEnumerable<UserActivityDto> ISyncResponseDto<UserActivityDto>.Entities => UserActivities;
    }
}