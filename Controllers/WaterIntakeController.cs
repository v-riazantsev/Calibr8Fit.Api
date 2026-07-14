using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.DataTransferObjects.WaterIntakeRecord;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    // Tracks user water intake records and synchronization
    [Route("api/water-intake")]
    [ApiController]
    [Authorize]
    public class WaterIntakeController(
        IUserSyncRepositoryBase<WaterIntakeRecord, Guid> waterIntakeRecordRepository,
        ICurrentUserService currentUserService,
        ISyncService<WaterIntakeRecord, Guid> syncService
        ) : SyncableEntityControllerBase<
        WaterIntakeRecord,
        WaterIntakeRecordDto,
        Guid,
        IUserSyncRepositoryBase<WaterIntakeRecord, Guid>,
        UpdateWaterIntakeRecordRequestDto,
        AddWaterIntakeRecordRequestDto,
        SyncWaterIntakeRecordRequestDto,
        SyncWaterIntakeRecordResponseDto
        >(
            currentUserService,
            waterIntakeRecordRepository,
            syncService,
            WaterIntakeRecordMapper.ToWaterIntakeRecordDto,
            WaterIntakeRecordMapper.ToWaterIntakeRecord,
            WaterIntakeRecordMapper.ToWaterIntakeRecord,
            WaterIntakeRecordMapper.ToSyncWaterIntakeRecordResponseDto
        )
    { }
}