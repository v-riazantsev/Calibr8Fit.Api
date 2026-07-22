using System.Security.Claims;
using Calibr8Fit.Api.Data;
using Calibr8Fit.Api.Services;
using Calibr8Fit.Api.Tests.Unit.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Calibr8Fit.Api.Tests.Unit.Services;

public class CurrentUserServiceTests
{
    private readonly CurrentUserService _currentUserService;

    public CurrentUserServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);

        context.Users.Add(TestUserFactory.CreateTestUser());
        context.SaveChanges();

        _currentUserService = new CurrentUserService(context);
    }

    [Theory]
    [InlineData("johndoe", true)]
    [InlineData("nonexistentuser", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public async Task GetCurrentUserAsync_ReturnsExpectedResult(
        string? username,
        bool shouldReturnUser)
    {
        var claimsPrincipal = username == null ?
            new ClaimsPrincipal() :
            new ClaimsPrincipal(
                new ClaimsIdentity(
                [
                    new Claim(
                        ClaimTypes.Name,
                        username)
                ],
                "mock"));

        var result = await _currentUserService
            .GetCurrentUserAsync(claimsPrincipal);

        if (shouldReturnUser)
        {
            Assert.NotNull(result);
            Assert.Equal(username, result!.UserName);
        }
        else
        {
            Assert.Null(result);
        }
    }
}