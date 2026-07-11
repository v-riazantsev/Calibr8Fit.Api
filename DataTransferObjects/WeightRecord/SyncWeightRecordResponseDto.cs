using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.WeightRecord
{
    public record SyncWeightRecordResponseDto : ISyncResponseDto<WeightRecordDto>
    {
        public required DateTime LastSyncedAt { get; init; }
        public required List<WeightRecordDto> WeightRecords { get; init; }

        IEnumerable<WeightRecordDto> ISyncResponseDto<WeightRecordDto>.Entities => WeightRecords;
    }

}
