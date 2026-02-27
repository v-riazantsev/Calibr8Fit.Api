using Calibr8Fit.Api.Interfaces.Model;

namespace Calibr8Fit.Api.Models
{
    public class Chat : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public bool IsGroupChat { get; set; } = false;
        public string? Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<ChatMember> Members { get; set; } = [];
        public virtual ICollection<ChatMessage> Messages { get; set; } = [];
    }
}