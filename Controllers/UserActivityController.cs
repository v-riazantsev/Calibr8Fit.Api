using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.DataTransferObjects.UserActivity;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    // Manages user-specific activity mappings and synchronization
    [Route("api/activity/my")]
    [ApiController]
    [Authorize]
    public class UserActivityController(
        ICurrentUserService currentUserService,
        IUserSyncRepositoryBase<UserActivity, Guid> userActivityRepository,
        ISyncService<UserActivity, Guid> syncService
        ) : SyncableEntityControllerBase<
            UserActivity,
            UserActivityDto,
            Guid,
            IUserSyncRepositoryBase<UserActivity, Guid>,
            UpdateUserActivityRequestDto,
            AddUserActivityRequestDto,
            SyncUserActivitiesRequestDto,
            SyncUserActivityResponseDto
        >(
            currentUserService,
            userActivityRepository,
            syncService,
            UserActivityMapper.ToUserActivityDto,
            UserActivityMapper.ToUserActivity,
            UserActivityMapper.ToUserActivity,
            UserActivityMapper.ToSyncUserActivityResponseDto
        )
    { }
}