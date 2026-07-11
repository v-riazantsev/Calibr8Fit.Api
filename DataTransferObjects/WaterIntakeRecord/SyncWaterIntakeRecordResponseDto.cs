using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.WaterIntakeRecord
{
    public record SyncWaterIntakeRecordResponseDto : ISyncResponseDto<WaterIntakeRecordDto>
    {
        public required DateTime LastSyncedAt { get; init; }
        public required List<WaterIntakeRecordDto> WaterIntakeRecords { get; init; }

        IEnumerable<WaterIntakeRecordDto> ISyncResponseDto<WaterIntakeRecordDto>.Entities => WaterIntakeRecords;
    }
}
