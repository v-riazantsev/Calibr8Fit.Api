using Calibr8Fit.Api.Interfaces.Model;

namespace Calibr8Fit.Api.Models
{
    public class ChatMessage : IUserEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public virtual Chat? Chat { get; set; }
        public required string UserId { get; set; }
        public virtual User? User { get; set; }
        public virtual ChatMember? SenderMembership { get; set; }

        public required string Content { get; set; }

        public required DateTime SentAt { get; set; }
        public virtual ICollection<ChatMessageRead> Reads { get; set; } = [];

        public bool Deleted { get; set; } = false;
    }
}
