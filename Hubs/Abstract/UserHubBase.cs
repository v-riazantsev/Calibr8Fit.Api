using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Models;
using Microsoft.AspNetCore.SignalR;

namespace Calibr8Fit.Api.Hubs.Abstract
{
    public abstract class UserHubBase(
        ICurrentUserService currentUserService
    ) : Hub
    {
        protected readonly ICurrentUserService _currentUserService = currentUserService;

        // Helper method to get the current user
        protected async Task<T> WithUser<T>(Func<User, T> action)
        {
            if (Context.User is null) throw new HubException("User is not authenticated.");
            var user = await _currentUserService.GetCurrentUserAsync(Context.User);
            if (user is null) throw new HubException("User not found.");
            return action(user);
        }

        protected async Task<T> WithUser<T>(Func<User, Task<T>> action)
        {
            if (Context.User is null) throw new HubException("User is not authenticated.");
            var user = await _currentUserService.GetCurrentUserAsync(Context.User);
            if (user is null) throw new HubException("User not found.");
            return await action(user);
        }

        protected async Task<T> WithUserId<T>(Func<string, T> action)
        {
            if (Context.User is null) throw new HubException("User is not authenticated.");
            var user = await _currentUserService.GetCurrentUserAsync(Context.User);
            if (user is null) throw new HubException("User not found.");
            return action(user.Id);
        }

        protected async Task<T> WithUserId<T>(Func<string, Task<T>> action)
        {
            if (Context.User is null) throw new HubException("User is not authenticated.");
            var user = await _currentUserService.GetCurrentUserAsync(Context.User);
            if (user is null) throw new HubException("User not found.");
            return await action(user.Id);
        }
    }
}
