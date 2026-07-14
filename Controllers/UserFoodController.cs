using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.DataTransferObjects.UserFood;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    // Manages user-specific food preferences and synchronization
    [Route("api/food/my")]
    [ApiController]
    public class UserFoodController(
        ICurrentUserService currentUserService,
        IUserSyncRepositoryBase<UserFood, Guid> userFoodRepository,
        ISyncService<UserFood, Guid> syncService
        ) : SyncableEntityControllerBase<
        UserFood,
        UserFoodDto,
        Guid,
        IUserSyncRepositoryBase<UserFood, Guid>,
        UpdateUserFoodRequestDto,
        AddUserFoodRequestDto,
        SyncUserFoodsRequestDto,
        SyncUserFoodResponseDto
        >(
            currentUserService,
            userFoodRepository,
            syncService,
            UserFoodMapper.ToUserFoodDto,
            UserFoodMapper.ToUserFood,
            UserFoodMapper.ToUserFood,
            UserFoodMapper.ToSyncUserFoodResponseDto
        )
    { }
}
