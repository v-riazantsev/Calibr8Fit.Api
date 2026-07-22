using System.Security.Claims;
using Calibr8Fit.Api.Data;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Calibr8Fit.Api.Services
{
    // Retrieves the current authenticated user from claims principal
    public class CurrentUserService(ApplicationDbContext context) : ICurrentUserService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<User?> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            var userName = user?.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
                return null;

            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }
    }
}