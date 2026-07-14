using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.DataTransferObjects.Activity;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    // Handles CRUD operations for global activities
    [Route("api/activity")]
    [ApiController]
    public class ActivityController(
        IDataVersionRepositoryBase<Activity, Guid> activityRepository
        ) : EntityControllerBase<
        Activity,
        ActivityDto,
        Guid,
        IDataVersionRepositoryBase<Activity, Guid>,
        UpdateActivityRequestDto,
        AddActivityRequestDto
        >(
            activityRepository,
            ActivityMapper.ToActivityDto,
            ActivityMapper.ToActivity,
            ActivityMapper.ToActivity
        )
    { }
}
