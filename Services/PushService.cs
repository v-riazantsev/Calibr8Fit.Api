using Calibr8Fit.Api.DataTransferObjects.PushToken;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Models;
using FirebaseAdmin.Messaging;

namespace Calibr8Fit.Api.Services
{
    // Manages push notification tokens and sends Firebase push notifications
    public class PushService(
        IUserRepositoryBase<PushToken, string[]> pushTokenRepository
    ) : IPushService
    {
        private readonly IUserRepositoryBase<PushToken, string[]> _pushTokenRepository = pushTokenRepository;

        public async Task RegisterPushToken(PushTokenDto pushTokenDto, string userId)
        {
            // Create new push token record
            var pushToken = new PushToken
            {
                UserId = userId,
                DeviceId = pushTokenDto.DeviceId,
                Token = pushTokenDto.Token,
                //Platform = pushTokenDto.Platform,
            };
            // Upsert the push token in the repository
            await _pushTokenRepository.AddOrUpdateAsync(pushToken);
        }

        public async Task PushNotificationAsync(string userId, string title, string body, string? imageUrl = null)
        {
            // Fetch all registered tokens for user and send multi-platform notifications
            var tokens = await _pushTokenRepository.GetAllByUserIdAsync(userId);

            if (tokens.Count == 0) return;

            var messages = tokens.Select(token => new Message
            {
                Token = token.Token,
                Notification = new Notification
                {
                    Title = title,
                    Body = body,
                    ImageUrl = imageUrl
                },

                Android = new AndroidConfig
                {
                    Notification = new AndroidNotification
                    {
                        ChannelId = "default",
                        ImageUrl = imageUrl
                    }
                },

                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Alert = new ApsAlert
                        {
                            Title = title,
                            Body = body
                        },
                        MutableContent = true
                    },
                    FcmOptions = new ApnsFcmOptions
                    {
                        ImageUrl = imageUrl
                    }
                }
            }).ToList();

            // Send the notifications
            await FirebaseMessaging.DefaultInstance.SendEachAsync(messages);
        }

    }
}