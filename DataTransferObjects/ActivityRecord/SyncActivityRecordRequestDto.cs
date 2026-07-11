using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.ActivityRecord
{
    public record SyncActivityRecordRequestDto : ISyncRequestDto<AddActivityRecordRequestDto>
    {
        public DateTime LastSyncedAt { get; init; } = DateTime.MinValue;
        public List<AddActivityRecordRequestDto> ActivityRecords { get; init; } = [];


        IEnumerable<AddActivityRecordRequestDto> ISyncRequestDto<AddActivityRecordRequestDto>.AddEntityRequestDtos => ActivityRecords;
    }
}