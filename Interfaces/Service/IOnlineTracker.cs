namespace Calibr8Fit.Api.Interfaces.Service
{
    public interface IOnlineTracker
    {
        void UserConnected(string username, string connectionId);
        void UserDisconnected(string connectionId);
        bool IsUserOnline(string username);
        HashSet<string> GetUserConnections(string username);
        string? GetUsernameByConnectionId(string connectionId);
    }
}