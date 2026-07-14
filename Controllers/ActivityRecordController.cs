using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.DataTransferObjects.ActivityRecord;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    // Records user activity sessions with validation and synchronization
    [Route("api/activity-record")]
    [ApiController]
    [Authorize]
    public class ActivityRecordController(
        IUserSyncRepositoryBase<ActivityRecord, Guid> activityRecordRepository,
        ICurrentUserService currentUserService,
        ISyncService<ActivityRecord, Guid> syncService,
        ITPHValidationService<Guid, Activity, UserActivity> activityValidationService
    ) : SyncableEntityControllerBase<
        ActivityRecord,
        ActivityRecordDto,
        Guid,
        IUserSyncRepositoryBase<ActivityRecord, Guid>,
        UpdateActivityRecordRequestDto,
        AddActivityRecordRequestDto,
        SyncActivityRecordRequestDto,
        SyncActivityRecordResponseDto
        >(
        currentUserService,
        activityRecordRepository,
        syncService,
        ActivityRecordMapper.ToActivityRecordDto,
        ActivityRecordMapper.ToActivityRecord,
        ActivityRecordMapper.ToActivityRecord,
        ActivityRecordMapper.ToSyncActivityRecordResponseDto
        )
    {
        private readonly ITPHValidationService<Guid, Activity, UserActivity> _activityValidationService = activityValidationService;

        [HttpPost("sync")]
        public override Task<IActionResult> Sync([FromBody] SyncActivityRecordRequestDto requestDto) =>
            WithUserId(async userId =>
            {
                // Validate activity record links
                foreach (var record in requestDto.ActivityRecords)
                {
                    // Check if activity exists
                    if (!await _activityValidationService.ValidateUserAccessAsync(userId, record.ActivityId))
                        return BadRequest($"Activity with id: {record.ActivityId} does not exist for user.");
                }

                return await base.Sync(requestDto);
            });
        [HttpPost]
        public override Task<IActionResult> Add([FromBody] AddActivityRecordRequestDto requestDto) =>
            WithUserId(async userId =>
            {
                // Validate activity record link
                if (!await _activityValidationService.ValidateUserAccessAsync(userId, requestDto.ActivityId))
                    return BadRequest($"Activity with id: {requestDto.ActivityId} does not exist for user.");

                return await base.Add(requestDto);
            });
        [HttpPut]
        public override Task<IActionResult> Update([FromBody] UpdateActivityRecordRequestDto requestDto) =>
            WithUserId(async userId =>
            {
                // Validate activity record link
                if (!await _activityValidationService.ValidateUserAccessAsync(userId, requestDto.ActivityId))
                    return BadRequest($"Activity with id: {requestDto.ActivityId} does not exist for user.");

                return await base.Update(requestDto);
            });
    }
}