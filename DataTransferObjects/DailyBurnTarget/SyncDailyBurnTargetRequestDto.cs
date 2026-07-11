using Calibr8Fit.Api.Interfaces.DataTransferObjects;

namespace Calibr8Fit.Api.DataTransferObjects.DailyBurnTarget
{
    public record SyncDailyBurnTargetRequestDto : ISyncRequestDto<AddDailyBurnTargetRequestDto>
    {
        public DateTime LastSyncedAt { get; init; } = DateTime.MinValue;
        public List<AddDailyBurnTargetRequestDto> DailyBurnTargets { get; init; } = [];

        IEnumerable<AddDailyBurnTargetRequestDto> ISyncRequestDto<AddDailyBurnTargetRequestDto>.AddEntityRequestDtos => DailyBurnTargets;
    }
}