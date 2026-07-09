namespace Calibr8Fit.Api.Interfaces.Service
{
    public interface IChatActivityTracker
    {
        void OpenChat(string username, string connectionId, Guid chatId);
        void CloseChat(string connectionId);

        bool HasUserOpenChat(string username, Guid chatId);

        void StartTyping(string username, string connectionId, Guid chatId);
        void StopTyping(string connectionId, Guid chatId);

        IReadOnlyCollection<string> GetTypingUsernames(Guid chatId, string exceptUsername);
    }
}