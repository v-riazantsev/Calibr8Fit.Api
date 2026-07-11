using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.ConsumptionRecord
{
    public record SyncConsumptionRecordResponseDto : ISyncResponseDto<ConsumptionRecordDto>
    {
        public required DateTime LastSyncedAt { get; init; }
        public required List<ConsumptionRecordDto> ConsumptionRecords { get; init; }
        IEnumerable<ConsumptionRecordDto> ISyncResponseDto<ConsumptionRecordDto>.Entities => ConsumptionRecords;
    }
}
