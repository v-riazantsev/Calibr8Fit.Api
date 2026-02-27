using Calibr8Fit.Api.Interfaces.Model;

namespace Calibr8Fit.Api.Models
{
    public class ChatMember : IUserEntity<(Guid, string)>
    {
        public Guid ChatId { get; set; }
        public virtual Chat? Chat { get; set; }
        public string UserId { get; set; } = null!;
        public virtual User? User { get; set; }
        public bool IsAdmin { get; set; } = false;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<ChatMessage> SentMessages { get; set; } = [];

        (Guid, string) IEntity<(Guid, string)>.Id => (ChatId, UserId);
    }
}