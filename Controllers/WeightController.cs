using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.DataTransferObjects.WeightRecord;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    // Tracks user weight records and synchronization
    [Route("api/weight")]
    [ApiController]
    [Authorize]
    public class WeightController(
        IUserSyncRepositoryBase<WeightRecord, Guid> weightRecordRepository,
        ICurrentUserService currentUserService,
        ISyncService<WeightRecord, Guid> syncService
        ) : SyncableEntityControllerBase<
        WeightRecord,
        WeightRecordDto,
        Guid,
        IUserSyncRepositoryBase<WeightRecord, Guid>,
        UpdateWeightRecordRequestDto,
        AddWeightRecordRequestDto,
        SyncWeightRecordRequestDto,
        SyncWeightRecordResponseDto
        >(
            currentUserService,
            weightRecordRepository,
            syncService,
            WeightRecordMapper.ToWeightRecordDto,
            WeightRecordMapper.ToWeightRecord,
            WeightRecordMapper.ToWeightRecord,
            WeightRecordMapper.ToSyncWeightRecordResponseDto
        )
    { }
}