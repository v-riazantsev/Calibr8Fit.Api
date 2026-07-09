using System.Collections.Concurrent;
using Calibr8Fit.Api.Interfaces.Service;

namespace Calibr8Fit.Api.Services
{
    public class OnlineTracker : IOnlineTracker
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();
        private readonly object _lock = new();

        public void UserConnected(string username, string connectionId)
        {
            lock (_lock)
            {
                if (!_userConnections.TryGetValue(username, out var connections))
                {
                    connections = [];
                    _userConnections[username] = connections;
                }

                connections.Add(connectionId);
            }
        }

        public void UserDisconnected(string connectionId)
        {
            lock (_lock)
            {
                // Remove the connectionId from the user's connections
                foreach (var kvp in _userConnections)
                {
                    if (kvp.Value.Remove(connectionId))
                    {
                        // If the user has no more connections, remove the user from the dictionary
                        if (kvp.Value.Count == 0)
                            _userConnections.TryRemove(kvp.Key, out _);
                        break;
                    }
                }
            }
        }

        public bool IsUserOnline(string username) =>
         _userConnections.TryGetValue(username, out var connections) && connections.Count > 0;

        public HashSet<string> GetUserConnections(string username) =>
            _userConnections.TryGetValue(username, out var connections) ? [.. connections] : [];

        public string? GetUsernameByConnectionId(string connectionId)
        {
            lock (_lock)
            {
                foreach (var kvp in _userConnections)
                {
                    if (kvp.Value.Contains(connectionId))
                        return kvp.Key;
                }
            }

            return null;
        }
    }
}