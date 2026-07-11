using Calibr8Fit.Api.Enums;

namespace Calibr8Fit.Api.DataTransferObjects.PushToken
{
    public record PushTokenDto
    {
        public required string Token { get; init; }
        public required string DeviceId { get; init; }
        //public required Platform Platform { get; init; }
    }
}