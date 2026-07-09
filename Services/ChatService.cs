using Calibr8Fit.Api.DataTransferObjects.Chat;
using Calibr8Fit.Api.DataTransferObjects.Chat.Read;
using Calibr8Fit.Api.Interfaces.Repository;
using Calibr8Fit.Api.Interfaces.Repository.Base;
using Calibr8Fit.Api.Interfaces.Service;
using Calibr8Fit.Api.Mappers;
using Calibr8Fit.Api.Models;
using Calibr8Fit.Api.Services.Results;

namespace Calibr8Fit.Api.Services
{
    public class ChatService(
        IChatRepository chatRepository,
        IUserRepositoryBase<ChatMember, (Guid, string)> chatMemberRepository,
        IChatMessagesRepository chatMessageRepository,
        IUserRepository userRepository,
        IPathService pathService,
        IOnlineTracker onlineTracker,
        IChatActivityTracker chatActivityTracker
    ) : IChatService
    {
        private readonly IChatRepository _chatRepository = chatRepository;
        private readonly IUserRepositoryBase<ChatMember, (Guid, string)> _chatMemberRepository = chatMemberRepository;
        private readonly IChatMessagesRepository _chatMessageRepository = chatMessageRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPathService _pathService = pathService;
        private readonly IOnlineTracker _onlineTracker = onlineTracker;
        private readonly IChatActivityTracker _chatActivityTracker = chatActivityTracker;

        public async Task<Chat> CreateDirectChatAsync(string userId, string otherUserId)
        {
            // Check if a direct chat already exists between the two users
            if (await _chatRepository.GetDirectChatBetweenUsersAsync(userId, otherUserId) is not null)
                throw new InvalidOperationException("A direct chat between these users already exists.");

            // Create new direct chat
            var chat = await _chatRepository.AddAsync(new Chat
            {
                IsGroupChat = false,
                Name = null,
                Members =
                [
                    new() { UserId = userId, IsAdmin = false },
                    new() { UserId = otherUserId, IsAdmin = false }
                ]
            }) ?? throw new Exception("Failed to create direct chat.");
            return chat;
        }

        public async Task<Result<SendChatMessageResultDto>> SendDirectMessageAsync(SendDirectMessageRequestDto requestDto, User sender, bool createChatIfNotExists = true)
        {
            // Try to get recipient user by username
            var recipientUser = await _userRepository.GetByUsernameAsync(requestDto.RecipientUsername);
            if (recipientUser is null)
                return Result<SendChatMessageResultDto>.Failure("Recipient user not found.");

            // Try to get existing direct chat between sender and recipient
            var chat = await _chatRepository.GetDirectChatBetweenUsersAsync(sender.Id, recipientUser.Id);
            if (chat is null)
            {
                if (!createChatIfNotExists)
                    return Result<SendChatMessageResultDto>.Failure("No existing chat between sender and recipient.");

                // Create new direct chat if it doesn't exist
                chat = await CreateDirectChatAsync(sender.Id, recipientUser.Id);
            }

            // Create new chat message
            var createdMessage = await _chatMessageRepository.AddAndReturnDetailedAsync(requestDto.ToChatMessage(sender.Id, chat.Id));
            if (createdMessage is null)
                return Result<SendChatMessageResultDto>.Failure("Failed to send message.");

            var chatMessageDto = createdMessage.ToChatMessageDto(sender.UserName!, _pathService);

            // Notify recipient and sender devices via SignalR
            // await _chatNotifier.NotifyDirectMessageAsync(
            //     recipientUsername: recipientUser.UserName!,
            //     message: chatMessageDto
            // );

            // TODO: Send push notification 

            return Result<SendChatMessageResultDto>.Success(
                new SendChatMessageResultDto
                {
                    Message = chatMessageDto,
                    RecipientUsernames = [recipientUser.UserName!]
                });
        }

        public async Task<Result<SendChatMessageResultDto>> SendChatMessageAsync(SendChatMessageRequestDto requestDto, User sender)
        {
            // Try to get existing chat by ID
            var chat = await _chatRepository.GetAsync(requestDto.ChatId);
            if (chat is null)
                return Result<SendChatMessageResultDto>.Failure("Chat not found.");

            // Check if sender is a member of the chat
            if (!await _chatMemberRepository.KeyExistsAsync(requestDto.ChatId, sender.Id))
                return Result<SendChatMessageResultDto>.Failure("Sender is not a member of the chat.");

            // Create new chat message
            var createdMessage = await _chatMessageRepository.AddAndReturnDetailedAsync(requestDto.ToChatMessage(sender.Id));
            if (createdMessage is null)
                return Result<SendChatMessageResultDto>.Failure("Failed to send message.");

            var chatMessageDto = createdMessage.ToChatMessageDto(sender.UserName!, _pathService);

            // Notify all chat members via SignalR
            // var memberUsernames = chat.Members.Select(m => m.User!.UserName!).ToList();
            // await _chatNotifier.NotifyChatMessageAsync(
            //     recipientUsernames: [.. memberUsernames.Where(u => u != sender.UserName!)],
            //     message: chatMessageDto
            // );

            var recipientUsernames = chat.Members
                .Where(m => m.UserId != sender.Id)
                .Select(m => m.User!.UserName!)
                .ToList();

            return Result<SendChatMessageResultDto>.Success(
                new SendChatMessageResultDto
                {
                    Message = chatMessageDto,
                    RecipientUsernames = recipientUsernames
                });
        }

        public async Task<Result<IEnumerable<ChatMessageDto>>> GetDirectMessagesAsync(
            string userId,
            string otherUsername,
            Guid? before = null,
            int? pageSize = null)
        {
            var otherId = await _userRepository.GetIdByUsernameAsync(otherUsername);
            if (otherId is null) return Result<IEnumerable<ChatMessageDto>>.Failure("Other user not found.");

            var messages = before.HasValue && pageSize.HasValue
                ? await _chatMessageRepository.GetDetailedDirectChatMessagesAsync(userId, otherId, before.Value, pageSize.Value)
                : await _chatMessageRepository.GetDetailedDirectChatMessagesAsync(userId, otherId);

            return Result<IEnumerable<ChatMessageDto>>.Success(messages.ToChatMessageDtos(userId, _pathService));
        }
        public async Task<Result<IEnumerable<ChatMessageDto>>> GetChatMessagesAsync(
            Guid chatId,
            string userId,
            Guid? before = null,
            int? pageSize = null
        )
        {
            Console.WriteLine($"GetChatMessagesAsync called with chatId: {chatId}, userId: {userId}, before: {before}, pageSize: {pageSize}");
            var messages = await _chatMessageRepository.GetDetailedChatMessagesAsync(chatId, userId, before, pageSize);

            return Result<IEnumerable<ChatMessageDto>>.Success(messages.ToChatMessageDtos(userId, _pathService));
        }

        public async Task<Result<IEnumerable<ChatPreviewDto>>> GetUserChatsAsync(string userId)
        {
            var chats = await _chatRepository.GetUserChatsWithDetailsAsync(userId);

            return Result<IEnumerable<ChatPreviewDto>>.Success(
                chats.ToChatPreviewDtos(_pathService, _chatActivityTracker, userId));
        }

        public async Task<Result<ChatReadResultDto>> UpdateChatReadAsync(
            Guid fromMessageId,
            string userId)
        {
            var fromMessage = await _chatMessageRepository.GetAsync(fromMessageId);

            if (fromMessage is null)
                return Result<ChatReadResultDto>.Failure("Message not found.");

            if (fromMessage.UserId == userId)
                return Result<ChatReadResultDto>.Failure("Cannot read your own messages.");

            var chatId = fromMessage.ChatId;

            var membership =
                await _chatMemberRepository.GetAsync(chatId, userId);

            if (membership is null)
                return Result<ChatReadResultDto>.Failure("User is not a chat member.");

            if (membership.LastReadMessage is not null &&
                membership.LastReadMessage.SentAt >= fromMessage.SentAt)
            {
                return Result<ChatReadResultDto>.Failure("Already read.");
            }

            var newlyRead =
                await _chatMessageRepository
                    .GetLatestUnreadMessagesForEachSenderBeforeAsync(
                        chatId,
                        userId,
                        fromMessage.SentAt);

            var updatedMembership = await _chatMessageRepository.MarkMessagesAsReadAsync(
                fromMessageId,
                userId);

            if (updatedMembership is null)
                return Result<ChatReadResultDto>.Failure("Failed to update read state.");

            return Result<ChatReadResultDto>.Success(
                new ChatReadResultDto
                {
                    ReaderUpdate = new ChatReadDto(
                        chatId,
                        fromMessage.SentAt),

                    SenderUpdates = newlyRead
                        .Select(m => new ChatReadNotificationDto
                        {
                            SenderUsername = m.User!.UserName!,
                            Read = new ChatReadDto(
                                chatId,
                                m.SentAt)
                        })
                        .ToList()
                });
        }

        public async Task<Result> EnsureUserIsChatMemberAsync(
            Guid chatId,
            string userId)
        {
            if (!await _chatMemberRepository.KeyExistsAsync(chatId, userId))
                return Result.Failure("User is not a member of the chat.");

            return Result.Success();
        }
    }
}