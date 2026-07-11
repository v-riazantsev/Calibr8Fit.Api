using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.WaterIntakeRecord
{
    public record SyncWaterIntakeRecordRequestDto : ISyncRequestDto<AddWaterIntakeRecordRequestDto>
    {
        public DateTime LastSyncedAt { get; init; } = DateTime.MinValue;
        public List<AddWaterIntakeRecordRequestDto> WaterIntakeRecords { get; init; } = [];

        IEnumerable<AddWaterIntakeRecordRequestDto> ISyncRequestDto<AddWaterIntakeRecordRequestDto>.AddEntityRequestDtos => WaterIntakeRecords;
    }
}
