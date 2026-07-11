using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.ActivityRecord
{
    public record SyncActivityRecordResponseDto : ISyncResponseDto<ActivityRecordDto>
    {
        public required DateTime LastSyncedAt { get; init; }
        public required List<ActivityRecordDto> ActivityRecords { get; init; }
        IEnumerable<ActivityRecordDto> ISyncResponseDto<ActivityRecordDto>.Entities => ActivityRecords;
    }
}