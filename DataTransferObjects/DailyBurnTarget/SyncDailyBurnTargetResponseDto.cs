using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.DailyBurnTarget
{
    public record SyncDailyBurnTargetResponseDto : ISyncResponseDto<DailyBurnTargetDto>
    {
        public required DateTime LastSyncedAt { get; init; }
        public required List<DailyBurnTargetDto> DailyBurnTargets { get; init; }
        IEnumerable<DailyBurnTargetDto> ISyncResponseDto<DailyBurnTargetDto>.Entities => DailyBurnTargets;
    }
}