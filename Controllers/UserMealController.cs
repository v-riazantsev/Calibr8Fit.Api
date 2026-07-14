using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.DataTransferObjects.UserMeal;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    // Manages user meal creation and synchronization
    [Route("api/user-meal")]
    [ApiController]
    [Authorize]
    public class UserMealController(
        IUserSyncRepositoryBase<UserMeal, Guid> userMealRepository,
        ICurrentUserService currentUserService,
        ISyncService<UserMeal, Guid> syncService
    ) : SyncableEntityControllerBase<
        UserMeal,
        UserMealDto,
        Guid,
        IUserSyncRepositoryBase<UserMeal, Guid>,
        UpdateUserMealRequestDto,
        AddUserMealRequestDto,
        SyncUserMealsRequestDto,
        SyncUserMealResponseDto
        >(
        currentUserService,
        userMealRepository,
        syncService,
        UserMealMapper.ToUserMealDto,
        UserMealMapper.ToUserMeal,
        UserMealMapper.ToUserMeal,
        UserMealMapper.ToSyncUserMealResponseDto
    )
    {

    }
}