using Calibr8Fit.Api.Controllers.Abstract;
using Calibr8Fit.Api.DataTransferObjects.Food;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calibr8Fit.Api.Controllers
{
    // Handles CRUD operations for global food items
    [Route("api/food")]
    [ApiController]
    public class FoodController(
        IDataVersionRepositoryBase<Food, Guid> foodRepository
        ) : EntityControllerBase<
        Food,
        FoodDto,
        Guid,
        IDataVersionRepositoryBase<Food, Guid>,
        UpdateFoodRequestDto,
        AddFoodRequestDto
        >(
            foodRepository,
            FoodMapper.ToFoodDto,
            FoodMapper.ToFood,
            FoodMapper.ToFood
    )
    { }
}