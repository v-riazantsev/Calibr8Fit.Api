using Calibr8Fit.Api.DataTransferObjects.Chat;
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
        IChatNotifier chatNotifier
    ) : IChatService
    {
        private readonly IChatRepository _chatRepository = chatRepository;
        private readonly IUserRepositoryBase<ChatMember, (Guid, string)> _chatMemberRepository = chatMemberRepository;
        private readonly IChatMessagesRepository _chatMessageRepository = chatMessageRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPathService _pathService = pathService;
        private readonly IChatNotifier _chatNotifier = chatNotifier;

        public async Task<Chat> CreateDirectChatAsync(string userId1, string userId2)
        {
            // Check if a direct chat already exists between the two users
            if (await _chatRepository.GetDirectChatBetweenUsersAsync(userId1, userId2) is not null)
                throw new InvalidOperationException("A direct chat between these users already exists.");

            // Create new direct chat
            var chat = await _chatRepository.AddAsync(new Chat
            {
                IsGroupChat = false,
                Name = null,
                Members =
                [
                    new() { UserId = userId1, IsAdmin = false },
                    new() { UserId = userId2, IsAdmin = false }
                ]
            }) ?? throw new Exception("Failed to create direct chat.");
            return chat;
        }

        public async Task<Result<ChatMessageDto>> SendDirectMessageAsync(SendDirectMessageRequestDto requestDto, User sender, bool createChatIfNotExists = true)
        {
            // Try to get recipient user by username
            var recipientUser = await _userRepository.GetByUsernameAsync(requestDto.RecipientUsername);
            if (recipientUser is null)
                return Result<ChatMessageDto>.Failure("Recipient user not found.");

            // Try to get existing direct chat between sender and recipient
            var chat = await _chatRepository.GetDirectChatBetweenUsersAsync(sender.Id, recipientUser.Id);
            if (chat is null)
            {
                if (!createChatIfNotExists)
                    return Result<ChatMessageDto>.Failure("No existing chat between sender and recipient.");

                // Create new direct chat if it doesn't exist
                chat = await CreateDirectChatAsync(sender.Id, recipientUser.Id);
            }

            // Create new chat message
            var createdMessage = await _chatMessageRepository.AddAsync(requestDto.ToChatMessage(sender.Id, chat.Id));
            if (createdMessage is null)
                return Result<ChatMessageDto>.Failure("Failed to send message.");

            var chatMessageDto = createdMessage.ToChatMessageDto();

            // Notify recipient and sender devices via SignalR
            await _chatNotifier.NotifyDirectMessageAsync(
                senderUsername: sender.UserName!,
                recipientUsername: recipientUser.UserName!,
                message: chatMessageDto
            );

            // TODO: Send push notification 

            return Result<ChatMessageDto>.Success(chatMessageDto);
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
                ? await _chatMessageRepository.GetDirectChatMessagesAsync(userId, otherId, before.Value, pageSize.Value)
                : await _chatMessageRepository.GetDirectChatMessagesAsync(userId, otherId);

            return Result<IEnumerable<ChatMessageDto>>.Success(messages.ToChatMessageDtos());
        }

        public async Task<Result<IEnumerable<ChatDto>>> GetDirectChatsAsync(string userId)
        {
            var chats = await _chatRepository.GetUserChatsAsync(userId);
            var chatDtos = chats.ToChatDtos(_pathService);
            return Result<IEnumerable<ChatDto>>.Success(chatDtos);
        }
    }
}