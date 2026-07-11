using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.WeightRecord
{
    public record SyncWeightRecordRequestDto : ISyncRequestDto<AddWeightRecordRequestDto>
    {
        public required DateTime LastSyncedAt { get; init; } = DateTime.MinValue;
        public required List<AddWeightRecordRequestDto> WeightRecords { get; init; } = [];

        IEnumerable<AddWeightRecordRequestDto> ISyncRequestDto<AddWeightRecordRequestDto>.AddEntityRequestDtos => WeightRecords;
    }

}
