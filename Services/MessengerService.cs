using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.DataTransferObjects.Chat.Read;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Services.Results;

namespace Calibr8Fit.Api.Services
{
    public class MessengerService(
        IChatService chatService,
        IChatNotifier chatNotifier,
        IUserRepository userRepository
    ) : IMessengerService
    {
        private readonly IChatService _chatService = chatService;
        private readonly IChatNotifier _chatNotifier = chatNotifier;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<SendChatMessageResultDto>> SendDirectMessageAsync(
            SendDirectMessageRequestDto requestDto,
            User sender)
        {
            // Try to get recipient user by username
            var recipientUser = await _userRepository.GetByUsernameAsync(requestDto.RecipientUsername);
            if (recipientUser is null)
                return Result<SendChatMessageResultDto>.Failure("Recipient user not found.");

            // Persist the message first, then fan out notifications.
            var result = await _chatService.SendDirectMessageAsync(requestDto, sender, createChatIfNotExists: true);

            if (!result.Succeeded)
                return result;

            var message = result.Data!.Message;

            // Direct messages notify both the sender and the recipient.
            await _chatNotifier.NotifyMessageSentAsync(sender.UserName!, message);
            await _chatNotifier.NotifyMessageIncomingAsync(recipientUser.Id, recipientUser.UserName!, message);

            return result;
        }

        public async Task<Result<SendChatMessageResultDto>> SendChatMessageAsync(
            SendChatMessageRequestDto requestDto,
            User sender)
        {
            var result = await _chatService.SendChatMessageAsync(requestDto, sender);

            if (!result.Succeeded)
                return result;

            var response = result.Data!;

            // Group messages are broadcast to every other chat member.
            await _chatNotifier.NotifyMessageSentAsync(sender.UserName!, response.Message);

            foreach (var username in response.RecipientUsernames)
            {
                var recipientUser = await _userRepository.GetByUsernameAsync(username);
                if (recipientUser is not null)
                {
                    await _chatNotifier.NotifyMessageIncomingAsync(recipientUser.Id, recipientUser.UserName!, response.Message);
                }
            }

            return result;
        }

        public async Task<Result<ChatReadResultDto>> ReadMessagesAsync(Guid fromMessageId, User user)
        {
            var result = await _chatService.UpdateChatReadAsync(fromMessageId, user.Id);

            if (!result.Succeeded)
                return result;

            // Read receipts are sent back to each sender that needs updating.
            foreach (var notification in result.Data!.SenderUpdates)
            {
                await _chatNotifier.NotifyMessagesReadAsync(notification.SenderUsername, notification.Read);
            }

            return result;
        }
    }
}