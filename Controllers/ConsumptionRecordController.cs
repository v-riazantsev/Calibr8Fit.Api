using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.DataTransferObjects.ConsumptionRecord;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    // Tracks food/meal consumption records with validation and synchronization
    [Route("api/consumption-record")]
    [ApiController]
    [Authorize]
    public class ConsumptionRecordController(
        IUserSyncRepositoryBase<ConsumptionRecord, Guid> consumptionRecordRepository,
        ICurrentUserService currentUserService,
        ISyncService<ConsumptionRecord, Guid> syncService,
        ITPHValidationService<Guid, Food, UserFood> foodValidationService
    ) : SyncableEntityControllerBase<
        ConsumptionRecord,
        ConsumptionRecordDto,
        Guid,
        IUserSyncRepositoryBase<ConsumptionRecord, Guid>,
        UpdateConsumptionRecordRequestDto,
        AddConsumptionRecordRequestDto,
        SyncConsumptionRecordRequestDto,
        SyncConsumptionRecordResponseDto
        >(
        currentUserService,
        consumptionRecordRepository,
        syncService,
        ConsumptionRecordMapper.ToConsumptionRecordDto,
        ConsumptionRecordMapper.ToConsumptionRecord,
        ConsumptionRecordMapper.ToConsumptionRecord,
        ConsumptionRecordMapper.ToSyncConsumptionRecordResponseDto
    )
    {
        private readonly ITPHValidationService<Guid, Food, UserFood> _foodValidationService = foodValidationService;

        [HttpPost("sync")]
        public override Task<IActionResult> Sync([FromBody] SyncConsumptionRecordRequestDto requestDto) =>
            WithUserId(async userId =>
            {
                // Validate consumption record links
                foreach (var record in requestDto.ConsumptionRecords)
                {
                    // Check if food exists
                    if (record.FoodId.HasValue)
                    {
                        if (!await _foodValidationService.ValidateUserAccessAsync(userId, record.FoodId.Value))
                            return BadRequest($"Food with id: {record.FoodId} does not exist for user.");
                        continue;
                    }

                    if (record.UserMealId.HasValue)
                        continue;

                    // If neither food nor meal is provided, return bad request
                    return BadRequest("Either FoodId or UserMealId must be provided.");
                }

                return await base.Sync(requestDto);
            });

        [HttpPost]
        public override Task<IActionResult> Add([FromBody] AddConsumptionRecordRequestDto requestDto) =>
            WithUserId(async userId =>
            {
                // Check if food exists
                if (requestDto.FoodId.HasValue)
                {
                    if (!await _foodValidationService.ValidateUserAccessAsync(userId, requestDto.FoodId.Value))
                        return BadRequest($"Food with id: {requestDto.FoodId} does not exist for user.");
                }
                else if (!requestDto.UserMealId.HasValue) // If neither food nor meal is provided, return bad request
                    return BadRequest("Either FoodId or UserMealId must be provided.");

                return await base.Add(requestDto);
            });
    }
}