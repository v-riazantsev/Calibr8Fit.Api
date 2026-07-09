using System.Collections.Concurrent;
using Calibr8Fit.Api.Interfaces.Service;

namespace Calibr8Fit.Api.Services
{
    public class ChatActivityTracker(
        IOnlineTracker onlineTracker
    ) : IChatActivityTracker
    {
        private readonly IOnlineTracker _onlineTracker = onlineTracker;

        private readonly ConcurrentDictionary<string, Guid> _connectionOpenChats = new();
        private readonly ConcurrentDictionary<string, HashSet<Guid>> _connectionTypingChats = new();

        private readonly object _lock = new();

        public void OpenChat(string username, string connectionId, Guid chatId)
        {
            _connectionOpenChats[connectionId] = chatId;
        }

        public void CloseChat(string connectionId)
        {
            _connectionOpenChats.TryRemove(connectionId, out _);
        }

        public bool HasUserOpenChat(string username, Guid chatId)
        {
            var connectionIds = _onlineTracker.GetUserConnections(username);

            return connectionIds.Any(connectionId =>
                _connectionOpenChats.TryGetValue(connectionId, out var openChatId) &&
                openChatId == chatId);
        }

        public void StartTyping(string username, string connectionId, Guid chatId)
        {
            lock (_lock)
            {
                if (!_connectionTypingChats.TryGetValue(connectionId, out var chats))
                {
                    chats = [];
                    _connectionTypingChats[connectionId] = chats;
                }

                chats.Add(chatId);
            }
        }

        public void StopTyping(string connectionId, Guid chatId)
        {
            lock (_lock)
            {
                if (!_connectionTypingChats.TryGetValue(connectionId, out var chats))
                    return;

                chats.Remove(chatId);

                if (chats.Count == 0)
                {
                    _connectionTypingChats.TryRemove(connectionId, out _);
                }
            }
        }

        public bool IsUserTypingInChat(string username, Guid chatId)
        {
            var connectionIds = _onlineTracker.GetUserConnections(username);

            return connectionIds.Any(connectionId =>
                _connectionTypingChats.TryGetValue(connectionId, out var chats) &&
                chats.Contains(chatId));
        }

        public IReadOnlyCollection<string> GetTypingUsernames(Guid chatId, string exceptUsername)
        {
            var typingUsernames = new HashSet<string>();

            foreach (var kvp in _connectionTypingChats)
            {
                var connectionId = kvp.Key;
                var chats = kvp.Value;

                if (
                    chats.Contains(chatId) &&
                    _onlineTracker.GetUsernameByConnectionId(connectionId) is string username &&
                    !string.Equals(username, exceptUsername, StringComparison.OrdinalIgnoreCase))
                {
                    typingUsernames.Add(username);
                }
            }

            return typingUsernames;
        }
    }
}