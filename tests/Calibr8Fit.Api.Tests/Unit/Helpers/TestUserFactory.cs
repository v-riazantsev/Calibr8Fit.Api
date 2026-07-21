using Calibr8Fit.Api.Models;

namespace Calibr8Fit.Api.Tests.Unit.Helpers
{
    public class TestUserFactory
    {
        public static User CreateTestUser() => new User
        {
            Email = "johndoe@example.com",
            UserName = "johndoe",
        };
    }
}