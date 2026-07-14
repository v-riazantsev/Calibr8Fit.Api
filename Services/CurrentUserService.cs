using System.Security.Claims;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Calibr8Fit.Api.Services
{
    // Retrieves the current authenticated user from claims principal
    public class CurrentUserService(UserManager<User> userManager) : ICurrentUserService
    {
        private readonly UserManager<User> _userManager = userManager;

        public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            var userName = user?.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return null;

            return await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }
    }
}