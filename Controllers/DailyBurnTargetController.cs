using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.DataTransferObjects.DailyBurnTarget;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    // Manages daily calorie burn targets per activity with validation and synchronization
    [Route("api/daily-burn-target")]
    [ApiController]
    [Authorize]
    public class DailyBurnTargetController(
        IUserSyncRepositoryBase<DailyBurnTarget, Guid> dailyBurnTargetRepository,
        ICurrentUserService currentUserService,
        ISyncService<DailyBurnTarget, Guid> syncService,
        ITPHValidationService<Guid, Activity, UserActivity> activityValidationService
    ) : SyncableEntityControllerBase<
        DailyBurnTarget,
        DailyBurnTargetDto,
        Guid,
        IUserSyncRepositoryBase<DailyBurnTarget, Guid>,
        UpdateDailyBurnTargetRequestDto,
        AddDailyBurnTargetRequestDto,
        SyncDailyBurnTargetRequestDto,
        SyncDailyBurnTargetResponseDto
        >(
        currentUserService,
        dailyBurnTargetRepository,
        syncService,
        DailyBurnTargetMapper.ToDailyBurnTargetDto,
        DailyBurnTargetMapper.ToDailyBurnTarget,
        DailyBurnTargetMapper.ToDailyBurnTarget,
        DailyBurnTargetMapper.ToSyncDailyBurnTargetResponseDto
        )
    {
        private readonly ITPHValidationService<Guid, Activity, UserActivity> _activityValidationService = activityValidationService;

        [HttpPost("sync")]
        public override Task<IActionResult> Sync([FromBody] SyncDailyBurnTargetRequestDto requestDto) =>
            WithUserId(async userId =>
            {
                // Validate daily burn target links
                foreach (var target in requestDto.DailyBurnTargets)
                {
                    // Check if activity exists
                    if (!await _activityValidationService.ValidateUserAccessAsync(userId, target.ActivityId))
                        return BadRequest($"Activity with id: {target.ActivityId} does not exist for user.");
                }

                return await base.Sync(requestDto);
            });

        [HttpPost]
        public override Task<IActionResult> Add([FromBody] AddDailyBurnTargetRequestDto requestDto) =>
            WithUserId(async userId =>
            {
                // Validate daily burn target link
                if (!await _activityValidationService.ValidateUserAccessAsync(userId, requestDto.ActivityId))
                    return BadRequest($"Activity with id: {requestDto.ActivityId} does not exist for user.");

                return await base.Add(requestDto);
            });

        [HttpPut]
        public override Task<IActionResult> Update([FromBody] UpdateDailyBurnTargetRequestDto requestDto) =>
            WithUserId(async userId =>
            {
                // Validate daily burn target link
                if (!await _activityValidationService.ValidateUserAccessAsync(userId, requestDto.ActivityId))
                    return BadRequest($"Activity with id: {requestDto.ActivityId} does not exist for user.");

                return await base.Update(requestDto);
            });
    }
}