using Calibr8Fit.Api.DataTransferObjects.User;

namespace Calibr8Fit.Api.DataTransferObjects.Chat
{
    public class ChatDto
    {
        public required Guid Id { get; set; }
        public required string? Name { get; set; }
        public required bool IsGroupChat { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required IEnumerable<UserSummaryDto> Members { get; set; }
    }
}