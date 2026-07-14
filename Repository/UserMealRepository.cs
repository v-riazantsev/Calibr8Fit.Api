using Calibr8Fit.Api.Data;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Repository.Base;

namespace Calibr8Fit.Api.Repository
{
    // Manages user meals with meal item composition persistence
    public class UserMealRepository(
        ApplicationDbContext context
    ) : UserSyncRepositoryBase<UserMeal, Guid>(context)
    {
        protected override void UpdateProperties(UserMeal existingUserMeal, UserMeal updatedUserMeal)
        {
            // Replace existing meal items with updated meal items
            _context.UserMealItems.RemoveRange(existingUserMeal.MealItems);
            _context.UserMealItems.AddRange(updatedUserMeal.MealItems);

            base.UpdateProperties(existingUserMeal, updatedUserMeal);
        }
    }
}
