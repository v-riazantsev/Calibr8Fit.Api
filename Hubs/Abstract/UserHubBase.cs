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
        protected async Task WithUser(Func<User, Task> action)
        {
            if (Context.User is null) throw new HubException("User is not authenticated.");
            var user = await _currentUserService.GetCurrentUserAsync(Context.User);
            if (user is null) throw new HubException("User not found.");
            await action(user);
        }

        protected async Task<T> WithUser<T>(Func<User, Task<T>> action)
        {
            if (Context.User is null) throw new HubException("User is not authenticated.");
            var user = await _currentUserService.GetCurrentUserAsync(Context.User);
            if (user is null) throw new HubException("User not found.");
            return await action(user);
        }

        protected async Task WithUserId(Func<string, Task> action)
        {
            if (Context.User is null) throw new HubException("User is not authenticated.");
            var user = await _currentUserService.GetCurrentUserAsync(Context.User);
            if (user is null) throw new HubException("User not found.");
            await action(user.Id);
        }

        protected async Task<T> WithUserId<T>(Func<string, Task<T>> action)
        {
            if (Context.User is null) throw new HubException("User is not authenticated.");
            var user = await _currentUserService.GetCurrentUserAsync(Context.User);
            if (user is null) throw new HubException("User not found.");
            return await action(user.Id);
        }
        protected async Task WithUserName(Func<string, Task> action)
        {
            if (Context.User is null) throw new HubException("User is not authenticated.");
            var user = await _currentUserService.GetCurrentUserAsync(Context.User);
            if (user is null) throw new HubException("User not found.");
            await action(user.UserName!);
        }
        protected async Task<T> WithUserName<T>(Func<string, Task<T>> action)
        {
            if (Context.User is null) throw new HubException("User is not authenticated.");
            var user = await _currentUserService.GetCurrentUserAsync(Context.User);
            if (user is null) throw new HubException("User not found.");
            return await action(user.UserName!);
        }
    }
}
