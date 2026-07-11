using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.ConsumptionRecord
{
    public record SyncConsumptionRecordRequestDto : ISyncRequestDto<AddConsumptionRecordRequestDto>
    {
        public DateTime LastSyncedAt { get; init; } = DateTime.MinValue;
        public List<AddConsumptionRecordRequestDto> ConsumptionRecords { get; init; } = [];

        IEnumerable<AddConsumptionRecordRequestDto> ISyncRequestDto<AddConsumptionRecordRequestDto>.AddEntityRequestDtos => ConsumptionRecords;
    }
}
